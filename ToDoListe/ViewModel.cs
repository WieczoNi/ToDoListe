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
            public object MyTask {  get; set; }
            public bool Erledigt {  get; set; }
            public int TabIndex {  get; set; }
        }

        private List<Saves> _checkbox = new List<Saves> { };


        public ObservableCollection<Saves> Checkboxes 
        {
            get
            {
                ObservableCollection<Saves> tempBox = new ObservableCollection<Saves>();
                foreach (Saves item in _checkbox)
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
                    Saves newItem = new Saves();
                    newItem = savedItems;
                    _checkbox.Add(newItem);
                    RaisePropertyChanged("Checkboxes");
                }
            }
            Umsortieren();
        }
        private void PerformAngehakt()
        { Umsortieren(); SaveTaskList(); }

        private void SaveTaskList()
        {
            var meineAufgaben = new List<Saves> { }; 
            var options = new JsonSerializerOptions() { IncludeFields = true, };
            foreach (Saves aufgabe in _checkbox)
            {
                var newItem = new Saves();
                newItem = aufgabe;
                meineAufgaben.Add(newItem);
            }
            string serial = JsonSerializer.Serialize<List<Saves>>(meineAufgaben, options);

            File.WriteAllText(@"path.json", serial); 
        }
        
        private void Umsortieren()
        {
            int anzahl = 0;
            foreach (Saves aufgabe in _checkbox) 
            {                                       
                if (aufgabe.Erledigt == true)     
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
        private RelayCommand delAllTasks;
        private RelayCommand angehakt;

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
        
        private void PerformAddNewTask()      
        {
            int anzahl = Checkboxes.Count;
            anzahl++;
            Saves einzutragen = new Saves();
            einzutragen.MyTask = aufgabeTextbox;
            einzutragen.TabIndex = anzahl;
            _checkbox.Add(einzutragen);
            Umsortieren();
            SaveTaskList();
            RaisePropertyChanged("Checkboxes");
        }

        private void PerformDelDoneTasks()  
        {
            List<int> delList = new List<int> { };
            foreach (Saves item in _checkbox)
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
    }
}

