using Google.Protobuf.WellKnownTypes;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    public class ServerInfo {
        private int _id;
        private bool _frozen;
        private bool _suspected;
        private string _type;

        public ServerInfo(int id, bool frozen, bool suspected, string type) {
            _id = id;
            _frozen = frozen;
            _suspected = suspected;
            _type = type;
        }

        public bool is_bank () {
            return _type.Equals("bank");
        }

        public int get_id () {
            return _id;
        }

        public bool is_frozen () {
            return _frozen;
        }

        public bool is_suspected() {
            return _suspected;
        }

    }
    public class ServerState {

        // constants
        private const string _FROZEN = "F";
        private const string _SUSPECTED = "S";
        private const string _BONEY = "boney";
        private const string _BANK = "bank";
        private const int _MAXLOGSIZE = 4096;

        // this process state
        private int _id;
        private string _url = "";
        private TimeSpan _delta;
        private bool _frozen;
        private int _coordinator = -1;
        public Object coordinatorLock = new();
        private DateTime _starting_time;

        // F list

        // other servers info
        // instead of the url store the grpc stub <<<
        private Dictionary<int, BankPaxosServerConnection> _banks = new Dictionary<int, BankPaxosServerConnection>();
        private Dictionary<int, CompareAndSwapService.CompareAndSwapServiceClient> _bonies = new Dictionary<int, CompareAndSwapService.CompareAndSwapServiceClient>();

        private Dictionary<int, ServerInfo[]> _timeslots_info = new Dictionary<int, ServerInfo[]>();

        private int _max_slots = 0;
        private int _current_slot = 0;
        public Object currentSlotLock = new();
        public Dictionary<int, int> _coordinators_dict = new(); //<slot, coordinator for that slot>

        //-------------------------------------------
        //------ Bank Paxos specific variables ------
        //-------------------------------------------
        private Dictionary<Tuple<int, int>, BankCommand> allCommands = new();
        public Object allCommandsLock = new();

        private HashSet<Tuple<int, int>> unordered = new();
        public Object unorderedLock = new();

        private Dictionary<int, Tuple<int, int>> ordered = new ();
        public Object orderedLock = new();

        private int lastApplied = -1;
        public Object lastAppliedLock = new();

        private int lastTentative = -1; //for the coordinator to know which number
                                       //to assign to the next command
        public Object lastTentativeLock = new();

        private int lastSequential = -1; //index of the last sequential command acked by the replica
        public Object lastSequentialLock = new();

        private int nextSequenceNumber = 0;
        public Object nextSequenceNumberLock = new();


        private List<FrozenInterceptor> _server_interceptors = new List<FrozenInterceptor>();



        public ServerState() {  }

        // getters
        public Dictionary<int, CompareAndSwapService.CompareAndSwapServiceClient> get_boney_servers () {
            return _bonies;
        }

        public bool is_frozen() {
            return _frozen;
        }

        public bool is_coordinator() {
            return _coordinator == _id;
        }

        public int get_coordinator_id () {
            return _coordinator;
        }

        public void set_coordinator_id(int coordinator) {
            _coordinator = coordinator;
        }

        public void add_coordinator_id(int slot, int coord) {
            _coordinators_dict[slot] = coord;
        }

        public int get_coordinator_id(int slot) {
            return _coordinators_dict[slot];
        }

        public int get_id() {
            return _id;
        }

        public string get_url() {
            return _url;
        }

        public TimeSpan get_delta () {
            return _delta;
        }

        public bool has_next_slot () {
            return _current_slot < _max_slots;
        }

        public DateTime get_starting_time() {
            return _starting_time;
        }

        public int get_current_slot () {
            return _current_slot;
        }

        public Dictionary<int, BankPaxosServerConnection> get_bank_servers() {
            return _banks;
        }

        public void add_server_interceptor  (FrozenInterceptor interceptor) {
            _server_interceptors.Add(interceptor);
        }

        public bool add_server (string sid, string _class, string url) {
            // convert id
            int id;
            if (!int.TryParse(sid, out id)) {
                return false;
            }

            // add this server info
            if (id == _id) {
                _url = url;
            }

            // add other server info
            bool added_server = false;
            if (_class.Equals(_BANK)) {
                BankPaxosServerConnection client = new BankPaxosServerConnection(url);
                _banks.Add(id, client);
                added_server = true;

            } else if (_class.Equals(_BONEY)) {

                
                GrpcChannel channel = GrpcChannel.ForAddress(url);
                CompareAndSwapService.CompareAndSwapServiceClient client =
                    new CompareAndSwapService.CompareAndSwapServiceClient(channel);
                
                _bonies.Add(id, client);
                added_server = true;
            }

            return added_server;
        }

        public bool set_id(int id) {
            _id = id;
            return true;
        }

        public bool set_starting_date(string starting_date) {
            _starting_time = Convert.ToDateTime(starting_date);
            return true;
        }

        public bool set_delta (string delta) {

            int _delta_int;
            if (!int.TryParse(delta, out _delta_int)) {
                return false;
            }

            _delta = TimeSpan.FromMilliseconds(_delta_int);
            return true;
        }

        public bool add_timeslot(string slot, string[] timeslot_info) {

            int _slot;
            if (!int.TryParse(slot, out _slot)) {
                return false;
            }


            int i = 0;
            int server_number = timeslot_info.Length;
            ServerInfo[] servers_info = new ServerInfo[server_number];

            foreach (string info in timeslot_info) {
                string[] info_splited = info.Split(')')[0].Split(", ");

                string id = info_splited[0];
                string frozen = info_splited[1];
                string suspected = info_splited[2];

                // conver values
                int server_id;
                if (!int.TryParse(id, out server_id)) {
                    return false;
                }

                bool server_frozen = frozen.Equals(_FROZEN);
                bool server_suspected = suspected.Equals(_SUSPECTED);

                // check server type
                string type = "bank";
                if (_bonies.ContainsKey(server_id)) {
                    type = _BONEY;
                }

                // add new entry
                ServerInfo server_info = new ServerInfo(server_id, server_frozen, server_suspected, type);
                servers_info[i] = server_info;
                i++;

            }

            _timeslots_info.Add(_slot, servers_info);
            return true;
        }

        public bool set_max_slots(string max_slots) {
            int int_max_slots;
            if (!int.TryParse(max_slots, out int_max_slots)) {
                return false;
            }

            _max_slots = int_max_slots;
            return true;
        }

        public void setup_timeslot () {

            lock (currentSlotLock) { 
                _current_slot++;
                Monitor.PulseAll(currentSlotLock);
            }            

            var current_slot = _timeslots_info[_current_slot];
            bool was_frozen = _frozen;
            int coordinator = int.MaxValue;
            int previous_coordinator = _coordinator;
            bool previous_coord_frozen = true;
            foreach (var server_info in current_slot) {
                int id = server_info.get_id();
                // check if the current process is frozen
                if (id == _id) {
                    _frozen = server_info.is_frozen();
                    // if not frozen and it has the smallest id, it must be the coordinator
                    if (!_frozen && coordinator > id) {
                        coordinator = id;
                    }

                    if (id == previous_coordinator) {
                        previous_coord_frozen = server_info.is_frozen();
                    }
                    continue;
                }


                if (id == previous_coordinator) { 
                    previous_coord_frozen = server_info.is_suspected();
                }

                // check if the bank processess is the coordinator or not
                // coordinator is the smallest process id not suspected
                if (server_info.is_bank() && !server_info.is_suspected() && coordinator > id) {
                    coordinator = id;
                }
            }

            //send preferably the last server as coordinator if it is unsuspected
            if (previous_coord_frozen){
                _coordinator = coordinator;
            } else {
                _coordinator = previous_coordinator;
            }
            Console.WriteLine("previous_frozen= " + previous_coord_frozen);
            Console.WriteLine("previous_coordinator= " + previous_coordinator);
            Console.WriteLine("_coordinator= " + _coordinator);

            _coordinators_dict.Add(_current_slot, _coordinator);

            // freeze/unfreeze channels
            if (was_frozen != _frozen) {
                // freeze/unfreeze client channels
                foreach (var server_connection in _banks.Values) {
                    server_connection.toggle_freeze();
                }

                // freeze/unfreeze server channels
                foreach (var interceptor in _server_interceptors) {
                    interceptor.toggle_freeze();
                }
            }
        }



        //=============================================================
        //====================== COMMAND LOGS =========================
        //=============================================================
        public void addNewCommand(BankCommand command) {
            allCommands.Add(command.getCommandId(), command);
            unordered.Add(command.getCommandId());
        }

        public void addUnorderedId(Tuple<int, int> commandId) {
            unordered.Add(commandId);
        }

        public void removeUnordered(Tuple<int, int> commandId) {
            unordered.Remove(commandId);
        }

        public void addOrdered(Tuple<int, int> commandId, int sequence_number) {
            if (sequence_number > lastTentative)
                lastTentative = sequence_number;
            ordered[sequence_number] = commandId;
        }

        public void addAcceptedCommand(Tuple<int, int> commandId, int sequence_number) {
            if (unordered.Contains(commandId)) {
                removeUnordered(commandId);
            } else {
                if (ordered.ContainsValue(commandId)) {
                    var item = ordered.First(kvp => kvp.Value.Equals(commandId));
                    ordered.Remove(item.Key);
                }
            }
            addOrdered(commandId, sequence_number);
        }


        public int get_last_applied() {
            return lastApplied;
        }
        public void setLastApplied(int newLastApplied) {
            lastApplied = newLastApplied;
        }


        public int get_last_tentative() {
            return lastTentative;
        }
        public void set_last_tentative(int newLastTentative) {
            lastTentative = newLastTentative;
        }


        public int getLastSequential()
        {
            return lastSequential;
        }
        public void setLastSequential(int newLastSequential) {
            lastSequential = newLastSequential;
        }

        public int getAndIncNextSequenceNumber() {
            nextSequenceNumber++;
            return nextSequenceNumber - 1;
        }

        public int getNextSequenceNumber() {
            return nextSequenceNumber;
        }

        public void setNextSequenceNumber(int n) {
            nextSequenceNumber = n;
        }

        public BankCommand get_command(int sequence_number) {
            if (ordered.ContainsKey(sequence_number))
                return allCommands[ordered[sequence_number]];
            return null;
        }

        public BankCommand get_command(Tuple<int, int> key) {
            if (allCommands.ContainsKey(key)) 
                return allCommands[key];
            return null;
        }

        public Dictionary<Tuple<int, int>, BankCommand> get_all_commands()
        {
            return allCommands;
        }

        public bool command_exists(Tuple<int, int> commandId) {
            return allCommands.ContainsKey(commandId);
        }

        public Tuple<int, int> get_ordered_command(int index) {
            if (ordered.ContainsKey(index))
                return ordered[index];
            return null;
        }

        public bool is_index_taken(int index) {
            return ordered.ContainsKey(index);
        }

    }
}
