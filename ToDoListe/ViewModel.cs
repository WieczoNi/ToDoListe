using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        private List<CheckBox> _checkbox = new List<CheckBox> { }; 


        public ObservableCollection<CheckBox> Checkboxes //private Liste _checkbox muss in öffentliche ObservableCollection umgewandelt werden, welche an die ListBox gebunden ist. tempBox ist hierbei nur ein proxy
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

        //Bereitstellung der nötigen Property changed Aufrufe, standartmäßig in INotifyPropertyChanged enthalten
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
                string savesJson = File.ReadAllText(@"path.json"); //Liest die in path.json gespeicherten werte ab, und schreibt sie in die Liste meineAufgaben
                var option = new JsonSerializerOptions() { IncludeFields = true }; //Im CodeBehind funktioniert das auch ohne die var option
                meineAufgaben = JsonSerializer.Deserialize<List<Saves>>(savesJson, option);

                foreach (Saves savedItems in meineAufgaben) //Die Liste wird in Check boxen umgewandelt, und anschließend _checkbox angehangen.
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
        { Umsortieren(); SaveTaskList(); }

        private void SaveTaskList()
        {
            var meineAufgaben = new List<Saves> { }; 
            var options = new JsonSerializerOptions() { IncludeFields = true, };
            foreach (CheckBox aufgabe in _checkbox) //schreibt alle Vorhandenen Checkboxen aus _checkbox in eine Liste, nach dem Format Saves. Wandelt also Checkboxen in <Saves> um.
            {
                var newItem = new Saves();
                newItem.Erledigt = aufgabe.IsChecked ?? false;
                newItem.MyTask = aufgabe.Content.ToString();
                newItem.Platzierung = aufgabe.TabIndex;
                meineAufgaben.Add(newItem);
            }
            string serial = JsonSerializer.Serialize<List<Saves>>(meineAufgaben, options);

            File.WriteAllText(@"path.json", serial); //<Saves> Liste wird in path.json geschrieben
        }
        
        private void Umsortieren()
        {
            int anzahl = 0;
            foreach (CheckBox aufgabe in _checkbox) //Gibt den Checkboxen aus _checkbox eine Nummerierung durch den TabIndex.
            {                                       //angehakte Checkboxen erhalten den TabIndex Ihre platzierung + nummer an Items in der Liste
                if (aufgabe.IsChecked == true)      //Ausserdem werden hier angekreuzte Checkboxen anders gefärbt
                {
                    aufgabe.TabIndex = aufgabe.TabIndex + _checkbox.Count;
                    aufgabe.Foreground = Brushes.DarkGray;
                    aufgabe.Background = Brushes.DarkGray;
                }
                else
                {
                    aufgabe.TabIndex = anzahl;
                    anzahl++;
                    aufgabe.Foreground = Brushes.Black;
                    aufgabe.Background = Brushes.Gainsboro;
                }
            }
            RaisePropertyChanged("Checkboxes");
        }

        private RelayCommand addNewTask;    //Folgendes sind die Ereignisfunktionen für alle 3 Knöpfe. Sie Rufen jeweil PerformDelDoneTasks, PerformAddNewTask oder PerformDelAllTasks auf
        private RelayCommand delDoneTasks;
        private RelayCommand delAllTasks;
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
        
        private void PerformAddNewTask()        //Eine neue Checkbox wird _checkbox hinzgefügt. aufgabeTextbox ist hierbei mit der Textbox aus der View im Modus OneWayToSource gebunden.
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

        private void PerformDelDoneTasks() //Die Funktion geht alle Gegenstände in _checkbox durch. Alle mit gehakter CheckBox werden in eine Liste eingetragen, und mit .RemoveAll gelöscht  
        {
            List<int> delList = new List<int> { };
            foreach (CheckBox item in _checkbox)
            {

                if (item.IsChecked == true)
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
            RaisePropertyChanged("Checkboxes");
            SaveTaskList();
            
        }
    }
}

