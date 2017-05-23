using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ProjectSpace
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //btn_launch_game_Click(null, null);
            //this.Close();
        }

        private void btn_launch_game_Click(object sender, RoutedEventArgs e)
        {
            var mainGame = new MainGame();
        }
    }
}