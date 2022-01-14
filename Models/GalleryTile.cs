using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Imperial_Commander_Editor
{
  public class GalleryTile : INotifyPropertyChanged
  {
    string _tileSide, _selectedExpansion;
    int _tileNumber;

    public string tileSide { get { return _tileSide; } set { _tileSide = value; PC(); } }
    public string selectedExpansion { get { return _selectedExpansion; } set { _selectedExpansion = value; PC(); } }
    public int tileNumber { get { return _tileNumber; } set { _tileNumber = value; PC(); } }
    public ImageSource source { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    public void PC( [CallerMemberName] string n = "" )
    {
      if ( !string.IsNullOrEmpty( n ) )
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( n ) );
    }

    public GalleryTile( int num, string expansion, string side )
    {
      tileSide = side;
      tileNumber = num;
      selectedExpansion = expansion;
    }
  }
}
