using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;
using System.Windows.Forms;
using Button = System.Windows.Controls.Button;

namespace _1712482_1712509_1712528
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		MediaPlayer _player = new MediaPlayer();
		DispatcherTimer _timer;
		int _lastIndex = -1;
		private IKeyboardMouseEvents _hook;
		int LoopState = 0;
		int RandomState = 0;
		bool IsPause = false;

		public MainWindow()
		{
			InitializeComponent();
			_player.MediaEnded += _player_MediaEnded;
			_timer = new DispatcherTimer();
			_timer.Interval = TimeSpan.FromSeconds(1);
			_timer.Tick += timer_Tick;

			// Dang ky su kien hook
			_hook = Hook.GlobalEvents();
			_hook.KeyUp += KeyUp_hook;
		}
	



		private void playButton_Click(object sender, RoutedEventArgs e)
		{
			if (playlistListBox.SelectedIndex >= 0)
			{
				_lastIndex = playlistListBox.SelectedIndex;
				PlaySelectedIndex(_lastIndex);
			}
			else
			{
				System.Windows.MessageBox.Show("No file selected!");
			}
		}

		private void PlaySelectedIndex(int i)
		{

			string filename = _fullPaths[i].FullName;
			var filename2 = _fullPaths[i].Name;
			var converter = new NameConverter();
			var shortname = converter.Convert(filename2, null, null, null);
			_player.Open(new Uri(filename, UriKind.Absolute));

			System.Threading.Thread.Sleep(500);
			var duration = _player.NaturalDuration.TimeSpan;
			var testDuration = new TimeSpan(0,0,0);
			_player.Position = testDuration;

			_player.Play();
			IsPause = false;
			_timer.Start();
			Button1.Width = 65;
			Button1.Height = 65;


			var Image2 = new BitmapImage(new Uri(@"Image/pause.png", UriKind.RelativeOrAbsolute));
			PlayIcon.Width = 60;
			PlayIcon.Height = 60;
			PlayIcon.Source = Image2;

		}

		private void _player_MediaEnded(object sender, EventArgs e)
		{
			int a = _fullPaths.Count;
			if (RandomState == 1)
			{
				if (LoopState == 1)
				{
					PlaySelectedIndex(_lastIndex);
				}
				else if (LoopState == 2)
				{
					Random r = new Random();
					_lastIndex = r.Next(0, a - 1);

					PlaySelectedIndex(_lastIndex);

				}
				else if (LoopState == 0)
				{
					if (_lastIndex < a)
					{
						Random r = new Random();
						_lastIndex = r.Next(0, a - 1);
						PlaySelectedIndex(_lastIndex);
					}
				}
			}
			else if (RandomState == 0)
			{
				if (LoopState == 1)
				{
					PlaySelectedIndex(_lastIndex);
				}
				else if (LoopState == 2)
				{

					if (_lastIndex == a - 1)
					{
						_lastIndex = 0;
						PlaySelectedIndex(_lastIndex);
					}
					else
					{
						_lastIndex++;
						PlaySelectedIndex(_lastIndex);
					}

				}
				else if (LoopState == 0)
				{
					_lastIndex++;
					if (_lastIndex < a)
					{

						PlaySelectedIndex(_lastIndex);
					}
				}
			}
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if (_player.Source != null)
			{
				if (_lastIndex < _fullPaths.Count)
				{
					var filename = _fullPaths[_lastIndex].Name;
					var converter = new NameConverter();
					var shortname = converter.Convert(filename, null, null, null);

					var currentPos = _player.Position.ToString(@"mm\:ss");
					var duration = _player.NaturalDuration.TimeSpan.ToString(@"mm\:ss");

					MusicPlaying.Text = String.Format($"{currentPos} / {duration} --- {shortname}");
					MusicPlaying.Foreground = Brushes.Yellow;
				
					
				}
			}
			else
				Title = "No file selected...";
		}
		int PlayClicked = 0;
		void PlayMusicButton_Click(object sender, RoutedEventArgs e)
		{
			if (PlayClicked == 0)
			{
				var source = new BitmapImage(new Uri(@"Image/play.png", UriKind.RelativeOrAbsolute));
				PlayIcon.Source = source;
				IsPause = true;
				_player.Pause();
				PlayClicked++;
			}
			else if (PlayClicked == 1)
			{
				var source = new BitmapImage(new Uri(@"Image/pause.png", UriKind.RelativeOrAbsolute));
				PlayIcon.Source = source;
				_player.Play();
				IsPause = false;
				PlayClicked--;
			}

		}


		BindingList<FileInfo> _fullPaths = new BindingList<FileInfo>();
		BindingList<string> _MusicPlaying = new BindingList<string>();
		private void addButton_Click(object sender, RoutedEventArgs e)
		{
			var screen = new Microsoft.Win32.OpenFileDialog();
			if (screen.ShowDialog() == true)
			{
				var info = new FileInfo(screen.FileName);
				_fullPaths.Add(info);
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			playlistListBox.ItemsSource = _fullPaths;
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{

		}

		private void KeyUp_hook(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//Ctrl+shift+D
			if (e.Control && e.Shift && (e.KeyCode == Keys.D))
			{
				_lastIndex++;
				if (RandomState == 1)
				{
					Random r = new Random();
					_lastIndex = r.Next(0, _fullPaths.Count - 1);
					PlaySelectedIndex(_lastIndex);
				}
				else
				{
					if (_lastIndex >= _fullPaths.Count)
					{
						_lastIndex = 0;

					}
					PlaySelectedIndex(_lastIndex);
				}
			}
			//Ctrl+shift+A
			if (e.Control && e.Shift && (e.KeyCode == Keys.A))
			{
				_lastIndex--;
				if (RandomState == 1)
				{
					Random r = new Random();
					_lastIndex = r.Next(0, _fullPaths.Count - 1);
					PlaySelectedIndex(_lastIndex);
				}
				else
				{
					if (_lastIndex < 0)
					{
						_lastIndex = _fullPaths.Count - 1;
					}
					PlaySelectedIndex(_lastIndex);
				}
			}
			//Ctrl+shift+D to pause or play
			if (e.Control && e.Shift && (e.KeyCode == Keys.S))
			{
				if (IsPause == false)
				{
					PlayIcon.Source = new BitmapImage(new Uri(@"Image/play.png", UriKind.RelativeOrAbsolute));
					_player.Pause();
					IsPause = true;
				}
				else
				{
					PlayIcon.Source = new BitmapImage(new Uri(@"Image/pause.png", UriKind.RelativeOrAbsolute));
					_player.Play();
					IsPause = false;
				}

			}
		}


		private void Window_Closing(object sender, CancelEventArgs e)
		{
			_hook.KeyUp -= KeyUp_hook;
			_hook.Dispose();

		}

		private void PlaylistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void Remove_Click(object sender, RoutedEventArgs e)
		{
			var Index = playlistListBox.SelectedIndex;
			if (Index == _lastIndex)
			{
				_player.Pause();
				Button1.Width = 0;
				Button1.Height = 0;
				PlayIcon.Source = null;
				MusicPlaying.Text = null;
			}
			_fullPaths.RemoveAt(Index);

		}
		int tempclick = 0;
		private void LoopSong_Click(object sender, RoutedEventArgs e)
		{
			if (tempclick == 0)
			{
				
				LoopState = 1;
				tempclick++;
				Loop.Source = new BitmapImage(new Uri(@"Image/repeat.png", UriKind.RelativeOrAbsolute));
			}
			else if (tempclick == 1)
			{
				
				LoopState = 2;
				tempclick++;
				Loop.Source = new BitmapImage(new Uri(@"Image/loop.png", UriKind.RelativeOrAbsolute));
			}
			else if (tempclick == 2)
			{
				LoopState = 0;
				tempclick -= 2;
				Loop.Source = new BitmapImage(new Uri(@"Image/NotRepeat.jpg", UriKind.RelativeOrAbsolute));
			}



		}
		int temp2click = 0;
		private void RandomSong_Click(object sender, RoutedEventArgs e)
		{
			if (temp2click == 0)
			{
				RandomState = 1;
				temp2click++;
				RandomIcon.Source = new BitmapImage(new Uri(@"Image/random.png", UriKind.RelativeOrAbsolute));
			}
			else
			{
				RandomIcon.Source = new BitmapImage(new Uri(@"Image/NotRandom.png", UriKind.RelativeOrAbsolute));
				RandomState = 0;
				temp2click--;
			}

		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			const string filename2 = "save.txt";
			var writer = new StreamWriter(filename2);

			writer.WriteLine(_lastIndex);
			writer.WriteLine(_fullPaths.Count);
			for (int i = 0; i < _fullPaths.Count; i++)
			{
				writer.WriteLine(_fullPaths[i].FullName);
			}
			writer.Close();
			System.Windows.MessageBox.Show("Saved!!!");
		}



		private void LoadPlayList_Click(object sender, RoutedEventArgs e)
		{
			var screen = new Microsoft.Win32.OpenFileDialog();
			if (screen.ShowDialog() == true)
			{
				var filename = screen.FileName;
				var Reader = new StreamReader(filename);
				_lastIndex = int.Parse(Reader.ReadLine());
				int n = int.Parse(Reader.ReadLine());
				if (_fullPaths.Count > 0)
					_fullPaths.Clear();
				for (int i = 0; i < n; i++)
				{
					var info = new FileInfo(Reader.ReadLine());
					_fullPaths.Add(info);
				}
				Reader.Close();
				System.Windows.MessageBox.Show("Playlist is loaded");
				PlaySelectedIndex(_lastIndex);
			}

		}

		private void RemovePlaylist_Click(object sender, RoutedEventArgs e)
		{
			if (_fullPaths.Count > 0)
			{
				_player.Pause();
				_fullPaths.Clear();
				Button1.Width = 0;
				Button1.Height = 0;
				PlayIcon.Source = null;
				MusicPlaying.Text = null;
			}
			else
			{
				System.Windows.MessageBox.Show("Playlist is empty");
			}
		}
	}
}
