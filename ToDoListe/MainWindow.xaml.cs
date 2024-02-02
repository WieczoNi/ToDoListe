using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace ToDoListe
{
    public class Saves // Model Klasse
    {
        public string MyTask { get; set; }
        public bool Erledigt { get; set; }
        public int Platzierung { get; set; }
    }

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        
        

        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists(@"path.json"))
            {
                ReadSavedTasks();
            }
        } 
    
        private void ReadSavedTasks()
        {
                List<Saves> meineAufgaben = new List<Saves>();
                string savesJson = File.ReadAllText(@"path.json");
                meineAufgaben = JsonSerializer.Deserialize<List<Saves>>(savesJson);

                foreach (Saves savedItems in meineAufgaben)
                {
                    CheckBox newItem = new CheckBox();

                    newItem.Content = savedItems.MyTask;
                    newItem.IsChecked = savedItems.Erledigt;
                    newItem.TabIndex = savedItems.Platzierung;
                    newItem.FontSize = 22;
                    newItem.Click += CheckBox_Checked;
                    aufgaben.Items.Add(newItem);
                }
                Umsortieren();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
       {

        }

        private void eintragen_Click(object sender, RoutedEventArgs e)
        {
            
            if (inhalt.Text != "Aufgabe") //Nur wenn der Standartname geändert wurde
            {

                // eine neue Liste mit aufgaben erzeugen
                AddNewTask();
               
            }
            //Zwischenspeichern
            SaveTaskList();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //Zwischenspeichern
            SaveTaskList();
            Umsortieren();
        }

        private void AddNewTask()
        {

            int anzahl = aufgaben.Items.Count;
            anzahl++;
            CheckBox einzutragen = new CheckBox();
            einzutragen.Content = inhalt.Text;
            einzutragen.TabIndex = anzahl;
            einzutragen.FontSize = 22;
            einzutragen.Click += CheckBox_Checked;
            aufgaben.Items.Add(einzutragen);
            Umsortieren();
        }

        private void SaveTaskList()
        {
            var meineAufgaben = new List<Saves> { };


            // Aufgabenliste aus der Ui durchgehen und einzelne Aufgaben übernehmen
            foreach (CheckBox aufgabe in aufgaben.Items)
            {
                var newItem = new Saves();
                newItem.Erledigt = aufgabe.IsChecked ?? false;
                newItem.MyTask = aufgabe.Content.ToString();
                newItem.Platzierung = aufgabe.TabIndex;
                meineAufgaben.Add(newItem);
            }

            File.WriteAllText(@"path.json", JsonSerializer.Serialize(meineAufgaben));
        }
        private void Umsortieren()
        {
            int anzahl = 0;
            foreach (CheckBox aufgabe in aufgaben.Items)
            {
                if (aufgabe.IsChecked == true)
                {
                    aufgabe.TabIndex = aufgabe.TabIndex + aufgaben.Items.Count;
                }
                else
                {
                    aufgabe.TabIndex = anzahl;
                    anzahl++;
                }
            }
            aufgaben.Items.SortDescriptions.Add(new SortDescription("TabIndex", ListSortDirection.Ascending));
        }
    }
}
