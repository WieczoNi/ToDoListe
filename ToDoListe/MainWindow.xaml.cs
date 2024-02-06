using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using ViewModel;



namespace ToDoListe
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            aufgaben.Items.SortDescriptions.Add(new SortDescription("TabIndex", ListSortDirection.Ascending)); //Bestimmt die Sortierung der ListBox. Kann man zwar theoretisch auch durch Umwege im ViewModel erzielen, aber das wäre nicht sehr effizient
        }
    }
}
