using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VideoTracksDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class MainPage : Page
    {

        MediaPlaybackItem playbackItem;
        
        int videoTrackCount =-1;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void createVideoTrackMenu() {
            Debug.WriteLine($"creating videoTrackMenu ");
            if (videoTrackCount > 1) {

                for (int i = 0; i < videoTrackCount; i++) {

                    MenuFlyoutItem item = new MenuFlyoutItem();
                    item.Text = $"Video {i+1}";
                    item.Tag = i;
                    item.Click += Item_Click;
                    videoTrack.Items.Add(item);

                }

            }
            else {
                Debug.WriteLine($"Cant create videoTrackMenu, vts not loaded  ");
                menuBarItem.Items.RemoveAt(1);
            }


        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"Item Click Event");
            int index = (int)(sender as FrameworkElement).Tag;
            changeVideoTrack(index);
        }

        private void changeVideoTrack(int index)
        {
            Debug.WriteLine($"changing videoTrack ");
            if (playbackItem != null && playbackItem.VideoTracks != null)
            {
                  
                    playbackItem.VideoTracks.SelectedIndex = index;
                    
                    Debug.WriteLine($"index set to : {index} ");
                
                }
        }

        async private void Open_File(object sender, RoutedEventArgs e)
        {
            try { await PickSingleVideoFile(); } catch (Exception ex) { Debug.WriteLine(ex.ToString()); }

        }
        async private System.Threading.Tasks.Task PickSingleVideoFile()
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".mkv");

            var file = await openPicker.PickSingleFileAsync();
            //Debug.WriteLine(file);
            // mediaPlayer is a MediaPlayerElement defined in XAML
            if (file != null)
            {
                var source = MediaSource.CreateFromStorageFile(file);
                playbackItem = new MediaPlaybackItem(source);
                playbackItem.VideoTracks.SelectedIndexChanged += VideoTracks_SelectedIndexChanged;
              
                mediaplayerElement.Source = playbackItem;

            }
            
        }

        private async void VideoTracks_SelectedIndexChanged(ISingleSelectMediaTrackList sender, object args)
        {
            Debug.WriteLine("videoTracks_SelectedIndexChanged");
            var videoTrackIndex = sender.SelectedIndex;
            Debug.WriteLine($"index : {videoTrackIndex} ");
            videoTrackCount = playbackItem.VideoTracks.Count;
            
            Debug.WriteLine($"VideoTrackCount VTSI : {playbackItem.VideoTracks.Count} "); //5
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                createVideoTrackMenu();
                //Make sure to call the mediaPlayerElement on the UI thread:
                if (mediaplayerElement.MediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
                    mediaplayerElement.MediaPlayer.StepForwardOneFrame();
            });
        }

    }
}




    

