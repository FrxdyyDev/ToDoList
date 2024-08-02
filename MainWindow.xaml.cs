using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ToDoList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private List<string> toDos = new List<string>();
        
        public MainWindow()
        {
            InitializeComponent();
            this.Background = new ImageBrush(new BitmapImage(new Uri("../../data/background.png", UriKind.Relative)));
            addAllToDos();
            nameToDoBox.KeyDown += new KeyEventHandler(keyPressed);
        }

        private void keyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddToListBtn_OnClick(null, null);
            }
        }

        private void addAllToDos()
        {
            listPanel.Children.Clear();
            List<string> content = new List<string>(File.ReadAllLines("../../data/ToDo.txt"));
            for (int i = 0; i < content.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(content[i]))
                {
                    CheckBox box = addToPanel(content[i].Split(':')[0], bool.Parse(content[i].Split(':')[1]));
                    if (bool.Parse(content[i].Split(':')[1]))
                    {
                        finishedToDo(null, null, box);
                    }
                    else
                    {
                        unfinishedToDo(null, null, box);
                    }
                }
            }
        }

        private void AddToListBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!toDos.Contains(nameToDoBox.Text) && nameToDoBox.Text != "Enter name...")
            {
                FileStream stream;
                if (!Directory.Exists("../../data"))
                {
                    Directory.CreateDirectory("../../data");
                }
                if (!File.Exists("../../data/ToDo.txt"))
                {
                    File.Create("../../data/ToDo.txt");
                }
                StreamWriter streamWriter = File.AppendText("../../data/ToDo.txt");
                streamWriter.WriteLine($"{nameToDoBox.Text}:false");
                streamWriter.Close();
                
                addToPanel(nameToDoBox.Text, false);
                addToListBtn.Focus();
                nameToDoBox.Text = "Enter name...";
                nameToDoBox.FontStyle = FontStyles.Italic;
                nameToDoBox.Foreground = new SolidColorBrush(Colors.DimGray);
            }
        }

        private CheckBox addToPanel(string toDo, bool finished)
        {
            CheckBox box = new CheckBox();
            box.Background = new SolidColorBrush(Colors.Transparent);
            box.Width = 300;
            box.Height = 50;
            box.Content = toDo;
            box.IsChecked = finished;
            box.HorizontalAlignment = HorizontalAlignment.Left;
            Canvas.SetLeft(box,15);
            box.FontSize = 20;
            box.Style = (Style)FindResource("CustomCheckBoxStyle");
            box.Checked += (sender, args) => finishedToDo(sender, args, box);
            box.Unchecked += (sender, args) => unfinishedToDo(sender, args, box);

            Button button = new Button();
            button.Background = new ImageBrush(new BitmapImage(new Uri("../../data/bin.png", UriKind.Relative)));
            button.Width = 40;
            button.Height = 37;
            button.BorderThickness = new Thickness(0);
            Canvas.SetLeft(button, 300);
            Canvas.SetTop(button, 7);
            button.Click += (sender, args) => removeToDo(sender, args, toDo);
            
            Panel panel = new Canvas();
            panel.Background = new ImageBrush(new BitmapImage(new Uri("../../data/button-background.png", UriKind.Relative)));
            panel.Width = 350;
            panel.Height = 50;
            
            panel.Children.Add(box);
            panel.Children.Add(button);
            
            listPanel.Children.Add(panel);
            return box;
        }

        private void removeToDo(object sender, EventArgs e, string toDo)
        {
            List<string> lines = new List<string>(File.ReadAllLines("../../data/ToDo.txt"));
            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines[i].Contains($"{toDo}:"))
                {
                    lines.Remove(lines[i]);
                    break;
                }
            }
            File.WriteAllLines("../../data/ToDo.txt", lines);
            addAllToDos();
        }

        private void finishedToDo(object sender, EventArgs e, CheckBox box)
        {
            TextBlock textBlock = new TextBlock();
            if (box.Content is TextBlock block)
            {
                textBlock.Text = block.Text;
                changeLines(block.Text, true);
            }else if (box.Content is string text)
            {
                textBlock.Text = text;
                changeLines(text, true);
            }
            textBlock.FontSize = 20;
            textBlock.Foreground = new SolidColorBrush(Colors.Black);
            var decoration = new TextDecoration
            {
                Location = TextDecorationLocation.Strikethrough,
                Pen = new Pen(Brushes.Black, 2)
            };
            textBlock.TextDecorations = new TextDecorationCollection { decoration } ;
                
            box.Content = textBlock;
        }

        private void unfinishedToDo(object sender, EventArgs e, CheckBox box)
        {
            if (box.Content is TextBlock block)
            {
                block.TextDecorations = null;
                changeLines(block.Text, false);
            }
        }

        private void changeLines(string searchedText, bool change)
        {
            List<string> lines = new List<string>(File.ReadAllLines("../../data/ToDo.txt"));
            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines[i].Contains($"{searchedText}:"))
                {
                    lines[i] = $"{searchedText}:{change}";
                    break;
                }
            }
            File.WriteAllLines("../../data/ToDo.txt", lines);
        }

        private void NameToDoBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (nameToDoBox.Text == "Enter name..." && nameToDoBox.FontStyle == FontStyles.Italic)
            {
                nameToDoBox.Text = "";
                nameToDoBox.FontStyle = FontStyles.Normal;
                nameToDoBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void NameToDoBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameToDoBox.Text))
            {
                nameToDoBox.Text = "Enter name...";
                nameToDoBox.FontStyle = FontStyles.Italic;
                nameToDoBox.Foreground = new SolidColorBrush(Colors.DimGray);
            }
        }
    }
}