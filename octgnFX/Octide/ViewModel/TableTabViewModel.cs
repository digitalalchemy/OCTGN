﻿namespace Octide.ViewModel
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Threading;

    using Octgn.DataNew.Entities;

    using Octide.Messages;

    public class TableTabViewModel : ViewModelBase
    {
        private double angle;

        private double zoom;

        private Vector offset;

        private string boardImage;

        private double boardWidth;

        private double boardHeight;

        private Thickness boardMargin;

        private ImageBrush background;

        private double width;

        private double height;

        private string cardBack;

        private double cardWidth;

        private double cardHeight;

        private readonly string[] backgroundStyles = new string[4] { "tile", "uniform", "uniformToFill", "stretch" };

        public double Angle
        {
            get
            {
                return this.angle;
            }
            set
            {
                if (value.Equals(this.angle))
                {
                    return;
                }
                this.angle = value;
                this.RaisePropertyChanged("Angle");
            }
        }

        public double Zoom
        {
            get
            {
                return this.zoom;
            }
            set
            {
                this.zoom = value;
                this.RaisePropertyChanged("Zoom");
            }
        }

        public Vector Offset
        {
            get
            {
                return this.offset;
            }
            set
            {
                this.offset = value;
                this.RaisePropertyChanged("Offset");
            }
        }

        public string BoardImage
        {
            get
            {
                return this.boardImage;
            }
            set
            {
                this.boardImage = value;
                this.RaisePropertyChanged("BoardImage");
            }
        }

        public double BoardWidth
        {
            get
            {
                return this.boardWidth;
            }
            set
            {
                this.boardWidth = value;
                this.RaisePropertyChanged("BoardWidth");
            }
        }

        public double BoardHeight
        {
            get
            {
                return this.boardHeight;
            }
            set
            {
                this.boardHeight = value;
                this.RaisePropertyChanged("BoardHeight");
            }
        }

        public Thickness BoardMargin
        {
            get
            {
                return this.boardMargin;
            }
            set
            {
                this.boardMargin = value;
                this.RaisePropertyChanged("BoardMargin");
            }
        }

        public ImageBrush Background
        {
            get
            {
                return this.background;
            }
            set
            {
                this.background = value;
                this.RaisePropertyChanged("Background");
            }
        }

        public string BackgroundPath
        {
            get
            {
                var def = ViewModelLocator.GameLoader.Game;
                if (def.Table == null) return String.Empty;
                return def.Table.Background;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value)) return;
                if (!File.Exists(value)) return;
                var def = ViewModelLocator.GameLoader.Game;
                if (def.Table == null) return;

                def.Table.Background = value;
                SetBackground();
                this.RaisePropertyChanged("BackgroundPath");
            }
        }

        public string BackgroundStyle
        {
            get
            {
                if (ViewModelLocator.GameLoader.ValidGame == false) return String.Empty;
                return BackgroundStyles.FirstOrDefault(x=>x == ViewModelLocator.GameLoader.Game.Table.BackgroundStyle);
            }
            set
            {
                if(BackgroundStyles.Any(x=>x == value) && ViewModelLocator.GameLoader.ValidGame)
                {
                    ViewModelLocator.GameLoader.Game.Table.BackgroundStyle = BackgroundStyles.FirstOrDefault(x=>x == value);
                    SetBackground();
                }
                this.RaisePropertyChanged("BackgroundStyle");
            }
        }

        public int Width
        {
            get
            {
                return ViewModelLocator.GameLoader.ValidGame ? ViewModelLocator.GameLoader.Game.Table.Width : 200;
            }
            set
            {
                if (ViewModelLocator.GameLoader.ValidGame)
                {
                    if (value > 4000) value = 4000;
                    if (value < 5) value = 5;
                    ViewModelLocator.GameLoader.Game.Table.Width = value;
                }

                this.RaisePropertyChanged("Width");
                if (ViewModelLocator.GameLoader.ValidGame)
                    CenterView(ViewModelLocator.GameLoader.Game);
            }
        }

        public int Height
        {
            get
            {
                return ViewModelLocator.GameLoader.ValidGame ? ViewModelLocator.GameLoader.Game.Table.Height : 200;
            }
            set
            {
                if (ViewModelLocator.GameLoader.ValidGame)
                {
                    if (value > 4000) value = 4000;
                    if (value < 5) value = 5;
                    ViewModelLocator.GameLoader.Game.Table.Height = value;
                }

                this.RaisePropertyChanged("Height");
                if (ViewModelLocator.GameLoader.ValidGame)
                    CenterView(ViewModelLocator.GameLoader.Game);
            }
        }

        public string CardBack
        {
            get
            {
                return this.cardBack;
            }
            set
            {
                this.cardBack = value;
                this.RaisePropertyChanged("CardBack");
            }
        }

        public double CardWidth
        {
            get
            {
                return this.cardWidth;
            }
            set
            {
                this.cardWidth = value;
                this.RaisePropertyChanged("CardWidth");
            }
        }

        public double CardHeight
        {
            get
            {
                return this.cardHeight;
            }
            set
            {
                this.cardHeight = value;
                this.RaisePropertyChanged("CardHeight");
            }
        }

        public string[] BackgroundStyles
        {
            get
            {
                return this.backgroundStyles;
            }
        }

        public TableTabViewModel()
        {
            Zoom = 1;
            Messenger.Default.Register<PropertyChangedMessage<Game>>(this, x => this.RefreshValues());
            Messenger.Default.Register<MouseWheelTableZoom>(this, OnMouseWheelTableZoom);
            this.RefreshValues();
        }

        internal void RefreshValues()
        {
            var def = ViewModelLocator.GameLoader.Game;
            if (def.Table == null) return;
            BoardWidth = def.Table.BoardPosition.Width;
            BoardHeight = def.Table.BoardPosition.Height;
            var pos = new Rect(
                def.Table.BoardPosition.X,
                def.Table.BoardPosition.Y,
                def.Table.BoardPosition.Width,
                def.Table.BoardPosition.Height);
            BoardMargin = new Thickness(pos.Left, pos.Top, 0, 0);
            BoardImage = def.Table.Board;
            Width = def.Table.Width;
            Height = def.Table.Height;
            CardBack = def.CardBack;
            CardWidth = def.CardWidth;
            CardHeight = def.CardHeight;

            CenterView(def);
            SetBackground();
            RaisePropertyChanged("BackgroundPath");
            RaisePropertyChanged("BackgroundStyle");
        }

        public void CenterView(Game game)
        {
            var tableDef = game.Table;
            Offset = new Vector(tableDef.Width / 2, tableDef.Height / 2);
        }

        internal void SetBackground()
        {
            if (!DispatcherHelper.UIDispatcher.CheckAccess())
            {
                DispatcherHelper.UIDispatcher.Invoke(new Action(this.SetBackground));
                return;
            }
            if (ViewModelLocator.GameLoader.Game == null || ViewModelLocator.GameLoader.Game.Table == null) return;
            var tableDef = ViewModelLocator.GameLoader.Game.Table;
            var bim = new BitmapImage();
            bim.BeginInit();
            bim.CacheOption = BitmapCacheOption.OnLoad;
            bim.UriSource = new Uri(tableDef.Background);
            bim.EndInit();

            var backBrush = new ImageBrush(bim);
            if (!String.IsNullOrWhiteSpace(tableDef.BackgroundStyle))
                switch (tableDef.BackgroundStyle)
                {
                    case "tile":
                        backBrush.TileMode = TileMode.Tile;
                        backBrush.Viewport = new Rect(0, 0, backBrush.ImageSource.Width, backBrush.ImageSource.Height);
                        backBrush.ViewportUnits = BrushMappingMode.Absolute;
                        break;
                    case "uniform":
                        backBrush.Stretch = Stretch.Uniform;
                        break;
                    case "uniformToFill":
                        backBrush.Stretch = Stretch.UniformToFill;
                        break;
                    case "stretch":
                        backBrush.Stretch = Stretch.Fill;
                        break;
                }
            Background = backBrush;
        }

        internal void OnMouseWheelTableZoom(MouseWheelTableZoom e)
        {
            double oldZoom = Zoom; // May be animated

            // Set the new zoom level
            if (e.Delta > 0)
                Zoom = oldZoom + 0.125;
            else if (oldZoom > 0.15)
                Zoom = oldZoom - 0.125;

            // Adjust the offset to center the zoom on the mouse pointer
            //double ratio = oldZoom - Zoom;
            //Offset += new Vector(e.Center.X * ratio, e.Center.Y * ratio);
        }
    }
}