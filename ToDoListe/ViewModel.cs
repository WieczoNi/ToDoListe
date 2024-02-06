using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace ViewModel
{

    public class ViewModel : ViewBase, INotifyPropertyChanged
    {

        public ViewModel()
        {
            ReadSavedTasks();
        }


        public class Saves
        {
            public string MyTask;
            public bool Erledigt;
            public int Platzierung;
        }


        private string _inhalt;

        public string inhalt
        {
            get { return _inhalt; }
            set { _inhalt = inhalt; }
        }
        private List<CheckBox> _checkbox = new List<CheckBox> { };


        public ObservableCollection<CheckBox> Checkboxes
        {
            get
            {
                ObservableCollection<CheckBox> tempBox = new ObservableCollection<CheckBox>();
                foreach (CheckBox item in _checkbox)
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
            List<Saves> meineAufgaben = new List<Saves>();
            if (File.Exists(@"path.json"))
            {
                string savesJson = File.ReadAllText(@"path.json");
                var option = new JsonSerializerOptions() { IncludeFields = true };
                meineAufgaben = JsonSerializer.Deserialize<List<Saves>>(savesJson, option);
                foreach (Saves savedItems in meineAufgaben)
                {
                    CheckBox newItem = new CheckBox();

                    newItem.Content = savedItems.MyTask;
                    newItem.IsChecked = savedItems.Erledigt;
                    newItem.TabIndex = savedItems.Platzierung;
                    newItem.FontSize = 22;
                    newItem.Click += CheckBox_Checked;
                    _checkbox.Add(newItem);
                    RaisePropertyChanged("Checkboxes");
                }
            }
            Umsortieren();
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Umsortieren();
            SaveTaskList();
        }

        private void SaveTaskList()
        {
            var meineAufgaben = new List<Saves> { };
            var options = new JsonSerializerOptions() { IncludeFields = true, };
            foreach (CheckBox aufgabe in _checkbox)
            {
                var newItem = new Saves();
                newItem.Erledigt = aufgabe.IsChecked ?? false;
                newItem.MyTask = aufgabe.Content.ToString();
                newItem.Platzierung = aufgabe.TabIndex;
                meineAufgaben.Add(newItem);
            }
            string serial = JsonSerializer.Serialize<List<Saves>>(meineAufgaben, options);

            File.WriteAllText(@"path.json", serial);
        }
        private void PerformDelDoneTasks()
        {
            int durchgang = 0;
            List<int> delList = new List<int> { };
            foreach (CheckBox item in _checkbox)
            {

                if (item.IsChecked == true)
                {
                    delList.Add(item.TabIndex);
                    
                }
                else { durchgang++; }
            }
            _checkbox.RemoveAll(r => delList.Any(a => a == r.TabIndex));
            RaisePropertyChanged("Checkboxes");
            SaveTaskList();
        }
        private void Umsortieren()
        {
            int anzahl = 0;
            foreach (CheckBox aufgabe in _checkbox)
            {
                if (aufgabe.IsChecked == true)
                {
                    aufgabe.TabIndex = aufgabe.TabIndex + _checkbox.Count;
                }
                else
                {
                    aufgabe.TabIndex = anzahl;
                    anzahl++;
                }


            }
            RaisePropertyChanged("Checkboxes");
        }

        private RelayCommand addNewTask;
        private RelayCommand delDoneTasks;
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
        private void PerformAddNewTask()
        {
            int anzahl = Checkboxes.Count;
            anzahl++;
            CheckBox einzutragen = new CheckBox();
            einzutragen.Content = aufgabeTextbox;
            einzutragen.TabIndex = anzahl;
            einzutragen.FontSize = 22;
            einzutragen.Click += CheckBox_Checked;
            _checkbox.Add(einzutragen);
            Umsortieren();
            SaveTaskList();
            RaisePropertyChanged("Checkboxes");
        }
    }
}

