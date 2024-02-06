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
            aufgaben.Items.SortDescriptions.Add(new SortDescription("TabIndex", ListSortDirection.Ascending));
        }
    }
}
