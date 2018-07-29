using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyConsole;
using Futronic.Devices.FS26;
using SourceAFIS.Simple;

namespace UserManagerDemo
{
    class UserManagerDemoProgram : Program
    {
        public static string PrintsFolderName = "Prints";

        static void Main(string[] args)
        {
            // Create directory for prints
            Directory.CreateDirectory(PrintsFolderName);

            new UserManagerDemoProgram().Run();
        }

        public UserManagerDemoProgram() : base(nameof(UserManagerDemo), true)
        {
            AddPage(new Welcome(this));
            AddPage(new UserList(this));
            AddPage(new CreateUser(this));
            AddPage(new UserDetails(this));
            AddPage(new AddFingerprint(this));
            AddPage(new IdentifyUser(this));



            SetPage<Welcome>();
        }

        public string SelectedUser { get; set; }

        public static IEnumerable<string> GetUsernames()
        {
            var users = Directory.GetDirectories(UserManagerDemoProgram.PrintsFolderName);

            foreach (var directory in users)
            {
                var username = directory.Substring(UserManagerDemoProgram.PrintsFolderName.Length + 1);

                yield return username;
            }
        }

        internal class Welcome : MenuPage
        {
            public Welcome(Program program) : base(nameof(Welcome), program)
            {
                this.Menu.Add("Show list of known Users", () => program.NavigateTo<UserList>());
                this.Menu.Add("Identify User by scanning fingerprint", () => program.NavigateTo<IdentifyUser>());

                this.Menu.Add("Exit", () => Environment.Exit(0));
            }
        }

        internal class UserList : MenuPage
        {
            private readonly Program _program;

            public UserList(Program program) : base("Current List of users", program)
            {
                _program = program;
            }

            public override void Display()
            {
                foreach (var username in GetUsernames())
                {
                    var menuItem = $"User '{username}'";

                    if (!this.Menu.Contains(menuItem))
                    {
                        this.Menu.Add(menuItem, () =>
                        {
                            ((UserManagerDemoProgram) _program).SelectedUser = username;
                            this.Program.NavigateTo<UserDetails>();
                        });
                    }
                }

                var createNew = "Create new user";

                if (!this.Menu.Contains(createNew))
                {
                    this.Menu.Add(createNew, () => _program.NavigateTo<CreateUser>());
                }

                base.Display();
            }
        }

        internal class UserDetails : MenuPage
        {
            public UserDetails(Program program) : base(nameof(UserDetails), program)
            {
                this.Menu.Add("Add Fingerprint", () => program.NavigateTo<AddFingerprint>());
            }
        }

        internal class CreateUser : Page
        {
            public CreateUser(Program program) : base("Create new user", program)
            {
            }

            public override void Display()
            {
                base.Display();

                var username = Input.ReadString("Please provide a username: ");

                Directory.CreateDirectory(Path.Combine(UserManagerDemoProgram.PrintsFolderName, username));

                Output.WriteLine(ConsoleColor.Green, $"User '{username}' added", username);

                Input.ReadString("Press enter to continue");

                this.Program.NavigateBack();
            }
        }

        internal class AddFingerprint : Page
        {
            public AddFingerprint(Program program) : base(nameof(AddFingerprint), program)
            {
            }

            public override void Display()
            {
                base.Display();

                var device = new DeviceAccessor().AccessFingerprintDevice();

                device.FingerDetected += (sender, args) => { HandleNewFingerprint(device.ReadFingerprint()); };

                device.StartFingerDetection();

                Output.WriteLine("Please place your finger on the device or press enter to cancel");

                Input.ReadString(string.Empty);
                device.Dispose();

                this.Program.NavigateBack();
            }

            private void HandleNewFingerprint(Bitmap bitmap)
            {
                Output.WriteLine("Finger detected. Saving...");

                var randomFilename = Path.GetRandomFileName().Replace('.', 'f') + ".bmp";
                var username = ((UserManagerDemoProgram) this.Program).SelectedUser;

                bitmap.Save(Path.Combine(UserManagerDemoProgram.PrintsFolderName, username, randomFilename));

                Output.WriteLine(ConsoleColor.DarkGreen, "Fingerprint registered");
            }
        }

        internal class IdentifyUser : Page
        {
            private AfisEngine _afis;

            public IdentifyUser(Program program) : base(nameof(IdentifyUser), program)
            {
                _afis = new AfisEngine();
            }

            public override void Display()
            {
                var allPersons = new List<Person>();

                var i = 0;

                // Create missing templates
                foreach (var username in GetUsernames())
                {
                    var person = new Person();
                    person.Id = i++;

                    var dataFolder = Path.Combine(PrintsFolderName, username);

                    var allBitmaps = Directory.GetFiles(dataFolder, "*.bmp", SearchOption.TopDirectoryOnly).Select(Path.GetFileName);
                    //var allPatterns = Directory.GetFiles(dataFolder, "*.min", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).ToList();

                    foreach (var bitmapFile in allBitmaps)
                    {
                        //var fingerprintId = Path.GetFileNameWithoutExtension(bitmapFile);

                        //var patternFile = $"{fingerprintId}.min";

                        Bitmap bitmap = new Bitmap(Path.Combine(dataFolder, bitmapFile));

                        Fingerprint fp = new Fingerprint();
                        fp.AsBitmap = bitmap;

                        person.Fingerprints.Add(fp);
                    }

                    _afis.Extract(person);
                    allPersons.Add(person);
                }


                var device = new DeviceAccessor().AccessFingerprintDevice();

                device.FingerDetected += (sender, args) =>
                {
                    device.StopFingerDetection();

                    Output.WriteLine("Finger detected, dont remove");
                    var readFingerprint = device.ReadFingerprint();

                    Output.WriteLine("Finger captured. Validation in progress");
                    ValidateFingerprint(readFingerprint, allPersons);

                    device.StartFingerDetection();
                };

                device.StartFingerDetection();

                Output.WriteLine("Please place your finger on the device or press enter to cancel");

                Input.ReadString(string.Empty);
                device.Dispose();

                this.Program.NavigateBack();

            }

            private void ValidateFingerprint(Bitmap bitmap, List<Person> allPersons)
            {
                var unknownPerson = new Person();
                var fingerprint = new Fingerprint();
                fingerprint.AsBitmap = bitmap;

                unknownPerson.Fingerprints.Add(fingerprint);
                _afis.Extract(unknownPerson);

                var matches = _afis.Identify(unknownPerson, allPersons);

                var persons = matches as Person[] ?? matches.ToArray();
                foreach (var person in persons)
                {
                    var personId = person.Id;

                    var user = GetUsernames().ToList().ElementAt(personId);

                    Output.WriteLine(ConsoleColor.DarkGreen, $"Matched with {user}!");
                }

                if (!persons.Any())
                {
                    Output.WriteLine(ConsoleColor.DarkRed, "No match!");
                }
            }
        }

    }
}
