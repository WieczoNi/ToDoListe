using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToDoListe2;
namespace ViewModel
{
    public class ViewModel : ViewBase, INotifyPropertyChanged
    {
        public ViewModel()
        {
            if (File.Exists(@"usedb.json")) { if (File.ReadAllText(@"usedb.json") == "true") { useDatabase = true; } else { useDatabase = false; } }
            ReadSavedTasks();
            Erinnerung();
        }
        public bool useDatabase { get; set; } //Gebunden an die Checkbox im View

        private List<_saves.Saves> _checkbox = new List<_saves.Saves> { };
        public ObservableCollection<_saves.Saves> Checkboxes //Konvertierung von Liste zu ObservabelCollection. Nicht die eleganteste Lösung, aber ObservableCollections haben Probleme beim Serialisieren, und Listen haben Probleme bei ItemSource
        {
            get
            {
                ObservableCollection<_saves.Saves> tempBox = new ObservableCollection<_saves.Saves>();
                foreach (_saves.Saves item in _checkbox)
                {
                    tempBox.Add(item);
                }
                return tempBox;
            }
            set { Checkboxes = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ReadSavedTasks()
        {
            if (useDatabase != true)
            {
                List<_saves.Saves> meineAufgaben = new List<_saves.Saves>();
                if (File.Exists(@"path.json"))
                {
                    string savesJson = File.ReadAllText(@"path.json");
                    var option = new JsonSerializerOptions() { IncludeFields = true };
                    meineAufgaben = JsonSerializer.Deserialize<List<_saves.Saves>>(savesJson, option);

                    foreach (_saves.Saves savedItems in meineAufgaben)
                    {
                        _saves.Saves newItem = new _saves.Saves();
                        newItem = savedItems;
                        _checkbox.Add(newItem);
                    }
                }
                Umsortieren();
            }
            else
            {
                using var db = new DatabaseModels.SavesContext();
                {
                    _checkbox = db.SavedDB.ToList<_saves.Saves>();
                    Umsortieren();
                }
            }
        }
        private void PerformAngehakt()
        { Umsortieren(); SaveTaskList(); }

        private void SaveTaskList()
        {
            if (useDatabase != true)
            {
                var meineAufgaben = new List<_saves.Saves> { };
                var options = new JsonSerializerOptions() { IncludeFields = true, };
                foreach (_saves.Saves aufgabe in _checkbox)
                {
                    var newItem = new _saves.Saves();
                    newItem = aufgabe;
                    meineAufgaben.Add(newItem);
                }
                string serial = JsonSerializer.Serialize<List<_saves.Saves>>(meineAufgaben, options);
                File.WriteAllText(@"path.json", serial);
            }
            else
            {
                using var db = new DatabaseModels.SavesContext();
                {
                    foreach (_saves.Saves saves in db.SavedDB)
                    {
                        db.Remove(saves);
                    }
                    foreach (_saves.Saves item in _checkbox)
                    {
                        db.Add(item);
                    }
                    db.SaveChanges();
                }
            }
        }

        private void Umsortieren()
        {
            int anzahl = 0;
            foreach (_saves.Saves aufgabe in _checkbox)
            {
                if (aufgabe.Erledigt == true)

                {
                    aufgabe.TabIndex = aufgabe.TabIndex + _checkbox.Count;
                    aufgabe.DueDateString = "Schon Fertig";
                    aufgabe.DueDate = DateTime.MaxValue;
                }
                else
                {
                    aufgabe.TabIndex = anzahl;
                    anzahl++;
                }

            }
            _checkbox.Sort((x, y) => DateTime.Compare(x.DueDate ?? DateTime.Now, y.DueDate ?? DateTime.Now));
            RaisePropertyChanged("Checkboxes");
            SaveTaskList();
        }

        private RelayCommand addNewTask;
        private RelayCommand delDoneTasks;
        private RelayCommand delAllTasks;
        private RelayCommand angehakt;
        private RelayCommand localDB;

        public ICommand DelDoneTasks
        {
            get
            {
                if (delDoneTasks == null)
                {
                    delDoneTasks = new RelayCommand(PerformDelDoneTasks);
                }

                return delDoneTasks;
            }
        }
        public ICommand LocalDB
        {
            get
            {
                if (localDB == null)
                {
                    localDB = new RelayCommand(PerformLocalDB);
                }

                return localDB;
            }
        }
        public ICommand Angehakt
        {
            get
            {
                if (angehakt == null)
                {
                    angehakt = new RelayCommand(PerformAngehakt);
                }

                return angehakt;
            }
        }
        public ICommand DelAllTasks
        {
            get
            {
                if (delAllTasks == null)
                {
                    delAllTasks = new RelayCommand(PerformDelAllTasks);
                }

                return delAllTasks;
            }
        }

        public ICommand AddNewTask
        {
            get
            {
                if (addNewTask == null)
                {
                    addNewTask = new RelayCommand(PerformAddNewTask);
                }

                return addNewTask;
            }
        }
        public string aufgabeTextbox { get; set; }
        public Nullable<DateTime> selectedDate { get; set; }
        private void PerformLocalDB() //Wenn die Checkbox geklickt wird, setze _checkbox zurück, und lese von der Datenbank, sollte IsChecked == true, ansonsten lese von der JSON
        {
            string messageBoxText = "Willst du deine Änderungen speichern?";
            string caption = "Speichern";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                _checkbox.Clear();
                ReadSavedTasks();

            }
            else
            {
                if (useDatabase != true)
                {
                    using var db = new DatabaseModels.SavesContext();
                    {
                        db.RemoveRange(db.SavedDB);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (File.Exists(@"path.json"))
                    {
                        File.Delete(@"path.json");
                    }
                }
                _checkbox.Clear();
                ReadSavedTasks();
            }

            if (useDatabase == true) { File.WriteAllText(@"usedb.json", "true"); }
            else { File.WriteAllText(@"usedb.json", "false"); }


        }
        private void PerformAddNewTask()
        {
            int anzahl = Checkboxes.Count;
            anzahl++;
            _saves.Saves einzutragen = new _saves.Saves();
            einzutragen.MyTask = aufgabeTextbox;
            einzutragen.TabIndex = anzahl;
            if (selectedDate != null)
            {
                einzutragen.DueDate = selectedDate;
                einzutragen.DueDateString = DateTime.Parse(selectedDate.ToString()).ToString("d/M/yyyy");
            }
            else { einzutragen.DueDateString = " Kein Zeitlimit"; einzutragen.DueDate = DateTime.Parse("30.12.2999"); }
            _checkbox.Add(einzutragen);
            Umsortieren();
            SaveTaskList();
            RaisePropertyChanged("Checkboxes");
        }
        private void PerformDelDoneTasks()
        {
            List<int> delList = new List<int> { };
            foreach (_saves.Saves item in _checkbox)
            {

                if (item.Erledigt == true)
                {
                    delList.Add(item.TabIndex);
                }
            }
            _checkbox.RemoveAll(r => delList.Any(a => a == r.TabIndex));
            RaisePropertyChanged("Checkboxes");
            SaveTaskList();
        }
        private void PerformDelAllTasks() //_checkbox wird geleert, und es wird gespeichert
        {
            _checkbox.Clear();
            SaveTaskList();
            RaisePropertyChanged("Checkboxes");
        }
        private void Erinnerung()
        {
            string caption = "Deine Aufgaben für Heute:";
            List<_saves.Saves> dueToday = _checkbox.Where(s => s.DueDate < DateTime.Now).ToList();
            foreach (_saves.Saves item in dueToday)
            {
                caption += Environment.NewLine + item.MyTask;
            }
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBox.Show(caption, "Deine Aufgaben", MessageBoxButton.OK, icon);
        }
    }
}