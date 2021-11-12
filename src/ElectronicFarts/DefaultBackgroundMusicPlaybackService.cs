using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace ElectronicFarts
{
    public class DefaultBackgroundMusicPlaybackService 
    {
        public DefaultBackgroundMusicPlaybackService(ContentManager contentManager)
        {
            Content = contentManager;
        }

        private ContentManager Content { get; set; }
        private Song CurrentSong { get; set; }
        
        public void StartBackgroundMusic(string assetName)
        {
            if (CurrentSong != null)
            {
                StopBackgroundMusic();
                CurrentSong.Dispose();
                CurrentSong = null;
            }
            
            
            Content.RootDirectory = "Content";
            CurrentSong = Content.Load<Song>(assetName);
            MediaPlayer.Play(CurrentSong);
        }

        public void PauseBackgroundMusic()
        {
            if (CurrentSong == null) return;
            if (MediaPlayer.State != MediaState.Playing) return;
            MediaPlayer.Pause();
        }

        public void StopBackgroundMusic()
        {
            if (CurrentSong == null) return;
            if (MediaPlayer.State == MediaState.Stopped) return;
            MediaPlayer.Stop();
        }

        public void ResumeBackgroundMusic()
        {
            if (CurrentSong == null) return;
            if (MediaPlayer.State == MediaState.Stopped) return;
            MediaPlayer.Stop();
        }

        public float Volume
        {
            get { return MediaPlayer.Volume; }
            set { MediaPlayer.Volume = value; }
        }
    }
}
