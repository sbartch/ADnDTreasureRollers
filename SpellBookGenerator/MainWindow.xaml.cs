using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpellBookGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpellLibrary _library;

        SpellLibrary Library { get => _library; set => _library = value; }

    public MainWindow()
        {
            InitializeComponent();
            Library = JsonConvert.DeserializeObject<SpellLibrary>(File.ReadAllText("Spells.json"));
        }
    }
}
