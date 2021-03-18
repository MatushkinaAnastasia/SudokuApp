using Microsoft.Win32;
using SudokuClient.GameUtils;
using SudokuClient.GameUtils.Events;
using SudokuClient.GameUtils.Messages;
using SudokuClient.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UtilsLibrary.Data;

namespace SudokuClient.Views
{
    /// <summary>
    /// Логика взаимодействия для SudokuWindow.xaml
    /// </summary>
    public partial class SudokuWindow : Window
    {
        private GameClient _gameClient;
        private const int Size = 9;
        private SudokuCell[,] _data;
        private TextBox[,] _tbs;

        public SudokuWindow(IPEndPoint ip)
        {
            InitializeComponent();
            DataContext = this;
            Address = ip.ToString();

            _gameClient = new GameClient();
            _gameClient.ConnectionSucceeded += GameClient_ConnectionSucceeded;
            _gameClient.ConnectionFailed += GameClient_ConnectionFailed;
            _gameClient.ConnectionRefused += GameClient_ConnectionRefused;
            _gameClient.ConnectionClosed += GameClient_ConnectionClosed;
            _gameClient.Disconnected += GameClient_Disconnected;
            _gameClient.Winned += GameClient_Winned;
            _gameClient.ElementChanged += GameClient_ElementChanged;
            _gameClient.Initialized += GameClient_Initialize;

            Closing += (s, e) => _gameClient.Disconnect();
            Loaded += (s, e) => _gameClient.Connect(ip);
        }

        private void GameClient_Winned(object sender, WinnedEventArgs e)
        {
            MessageBox.Show("Победа! Судоку разгадано!");
        }

        public string Address { get; set; }

        private void GameClient_ConnectionClosed(object sender, ConnectionClosedEventArgs e)
        {
            MessageBox.Show("Связь с сервером потеряна. Вы можете сохранить игру.");
        }

        private void GameClient_Initialize(object sender, InitializeEventArgs e)
        {
            _data = e.Data;
            Dispatcher.Invoke(() =>
            {
                _tbs = grid.InitializeButtonsGrid(ValueChanging);
                FillTextBoxesWithData();
                Title = $"Комната {e.RoomName}";
            });
        }

        private void GameClient_ElementChanged(object sender, ElementChangedEventArgs e)
        {
            var x = e.X;
            var y = e.Y;
            var value = e.Value;
            Dispatcher.Invoke(() =>
            {
                _data[x, y].Value = value;
                _tbs[x, y].Text = value == 0 ? string.Empty : value.ToString();
            });
        }

        private void GameClient_Disconnected(object sender, DisconnectedEventArgs e)
        {
            //MessageBox.Show("GameClient_Disconnected");
        }

        private void GameClient_ConnectionRefused(object sender, ConnectionRefusedEventArgs e)
        {
            MessageBox.Show("Сервер разорвал соединение. Вы можете сохранить игру.");
        }

        private void GameClient_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            MessageBox.Show("Не удалось присоединиться.");
            Close();
        }

        private void GameClient_ConnectionSucceeded(object sender, ConnectionSucceededEventArgs e)
        {
            //MessageBox.Show("GameClient_ConnectionSucceeded");
        }

        private void BackToMenu(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FillTextBoxesWithData()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    var tb = _tbs[i, j];

                    SudokuCell cell = _data[i, j];
                    if (cell.Value == 0)
                    {
                        tb.Text = string.Empty;
                    }
                    else if (cell.IsPermanent)
                    {
                        tb.Text = cell.Value.ToString();
                        tb.IsEnabled = false;
                        tb.Foreground = Brushes.Black;
                        tb.FontWeight = FontWeights.UltraBold;
                    }
                    else if (cell.Value > 0 && cell.Value <= 9)
                    {
                        tb.Text = cell.Value.ToString();
                    }
                    else
                    {
                        tb.Text = string.Empty;
                    }
                }
            }
        }

        private readonly Dictionary<Key, byte> _keys = new Dictionary<Key, byte>()
        {
            { Key.D1, 1 },
            { Key.D2, 2 },
            { Key.D3, 3 },
            { Key.D4, 4 },
            { Key.D5, 5 },
            { Key.D6, 6 },
            { Key.D7, 7 },
            { Key.D8, 8 },
            { Key.D9, 9 },
            { Key.NumPad1, 1 },
            { Key.NumPad2, 2 },
            { Key.NumPad3, 3 },
            { Key.NumPad4, 4 },
            { Key.NumPad5, 5 },
            { Key.NumPad6, 6 },
            { Key.NumPad7, 7 },
            { Key.NumPad8, 8 },
            { Key.NumPad9, 9 },
        };

        private void ValueChanging(TextBox textbox, KeyEventArgs e)
        {
            var parameters = textbox.Tag as TextBoxParametres;
            e.Handled = true;

            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                _gameClient.SendMessage(new ChangeElementMessage(parameters.X, parameters.Y, 0));
            }
            else if ((int)e.Key >= (int)Key.D1 && (int)e.Key <= (int)Key.D9 || (int)e.Key >= (int)Key.NumPad1 && (int)e.Key <= (int)Key.NumPad9)
            {
                var value = _keys[e.Key];
                _gameClient.SendMessage(new ChangeElementMessage(parameters.X, parameters.Y, value));
            } /*else ignore*/
        }

        private void AddressButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Address);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Sudoku file (*.sudoku)|*.sudoku"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                var data = Encoding.UTF8.GetString(_data.ConvertToByteArray());
                File.WriteAllText(saveFileDialog.FileName, data);
            }
        }
    }
}
