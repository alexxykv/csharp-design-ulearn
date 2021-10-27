using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using FractalPainting.Infrastructure.Common;
using FractalPainting.Infrastructure.UiActions;

namespace FractalPainting.App
{
    public class ImageSettingsAction : IUiAction
    {
        public MenuCategory Category => MenuCategory.Settings;
        public string Name => "Изображение...";
        public string Description => "Размеры изображения";

        private readonly ImageSettings imageSettings;
        private readonly IImageHolder imageHolder;

        public ImageSettingsAction(ImageSettings imageSettings, IImageHolder imageHolder)
        {
            this.imageSettings = imageSettings;
            this.imageHolder = imageHolder;
        }

        public void Perform()
        {
            SettingsForm.For(imageSettings).ShowDialog();
            imageHolder.RecreateImage(imageSettings);
        }
    }

    public class SaveImageAction : IUiAction
    {
        public MenuCategory Category => MenuCategory.File;
        public string Name => "Сохранить...";
        public string Description => "Сохранить изображение в файл";

        private readonly AppSettings appSettings;
        private readonly IImageHolder imageHolder;

        public SaveImageAction(AppSettings appSettings, IImageHolder imageHolder)
        {
            this.appSettings = appSettings;
            this.imageHolder = imageHolder;
        }

        public void Perform()
        {
            var dialog = new SaveFileDialog
            {
                CheckFileExists = false,
                InitialDirectory = Path.GetFullPath(appSettings.ImagesDirectory),
                DefaultExt = "bmp",
                FileName = "image.bmp",
                Filter = "Изображения (*.bmp)|*.bmp"
            };
            var res = dialog.ShowDialog();
            if (res == DialogResult.OK)
                imageHolder.SaveImage(dialog.FileName);
        }
    }

    public class PaletteSettingsAction : IUiAction
    {
        public MenuCategory Category => MenuCategory.Settings;
        public string Name => "Палитра...";
        public string Description => "Цвета для рисования фракталов";

        private readonly Palette pallete;

        public PaletteSettingsAction(Palette pallete)
        {
            this.pallete = pallete;
        }

        public void Perform()
        {
            SettingsForm.For(pallete).ShowDialog();
        }
    }

    public class MainForm : Form
    {
        //public MainForm()
        //    : this(
        //        new IUiAction[]
        //        {
        //            new SaveImageAction(Services.GetAppSettings(), Services.GetImageHolder()),
        //            new DragonFractalAction(),
        //            new KochFractalAction(),
        //            new ImageSettingsAction(Services.GetImageSettings(), Services.GetImageHolder()),
        //            new PaletteSettingsAction(Services.GetPalette())
        //        }, Services.GetPictureBoxImageHolder())
        //{ }

        public MainForm(IUiAction[] actions, PictureBoxImageHolder pictureBox)
        {
            var imageSettings = CreateSettingsManager().Load().ImageSettings;
            ClientSize = new Size(imageSettings.Width, imageSettings.Height);

            pictureBox.RecreateImage(imageSettings);
            pictureBox.Dock = DockStyle.Fill;
            Controls.Add(pictureBox);

            var mainMenu = new MenuStrip();
            mainMenu.Items.AddRange(actions.ToMenuItems());
            mainMenu.Dock = DockStyle.Top;
            Controls.Add(mainMenu);
        }

        private static SettingsManager CreateSettingsManager()
        {
            return new SettingsManager(new XmlObjectSerializer(), new FileBlobStorage());
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Text = "Fractal Painter";
        }
    }
}
