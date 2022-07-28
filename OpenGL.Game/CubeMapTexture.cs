using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace OpenGL.Game
{
    public class CubeMapTexture : IDisposable
    {
        #region Propreties

        public string[] Filenames { get; private set; }

        public uint TextureID { get; private set; }

        public Size Size { get; private set; }

        public TextureTarget TextureTarget { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a texture from the supplied filename.
        /// Any files that Bitmap.FromFile can open are supported.
        /// This method also supports dds textures (as long as the file extension is .dds).
        /// </summary>
        /// <param name="filenames">The path to the texture to load.</param>
        public CubeMapTexture(string[] filenames)
        {
            foreach (string s in filenames)
            {
                if (!File.Exists(s))
                {
                    throw new FileNotFoundException(string.Format("The file {0} does not exist.", s));
                }
            }

            TextureTarget = TextureTarget.TextureCubeMap;
            TextureID = Gl.GenTexture();
            Gl.BindTexture(TextureTarget, TextureID); // bind the texture to memory in OpenGL
            
            Gl.TexParameteri(TextureTarget, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            Gl.TexParameteri(TextureTarget, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
            Gl.TexParameteri(TextureTarget, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            Gl.TexParameteri(TextureTarget, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            Gl.TexParameteri(TextureTarget, TextureParameterName.TextureWrapR, TextureParameter.ClampToEdge);
            
            Filenames = filenames;

            int i = 2;
            foreach (string path in filenames) 
            {
                LoadBitmap(path, i, false);
                i++;
            }

            Gl.BindTexture(TextureTarget, 0);
        }


        ~CubeMapTexture()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (TextureID != 0)
            {
                Gl.DeleteTexture(TextureID);
                TextureID = 0;
            }
        }

        private void LoadBitmap(string filename, int offset, bool flipy = true)
        {
            using (Bitmap image = (Bitmap) Image.FromFile(filename))
            {
                Size = new Size(image.Width, image.Height);

                if (flipy)
                {
                    image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }

                // must be Format32bppArgb file format, so convert it if it isn't in that format
                BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                Gl.TexImage2D(TextureTarget + offset, 0, PixelInternalFormat.Rgba8, image.Width, image.Height, 0,
                    PixelFormat.Bgra, PixelType.UnsignedByte, imageData.Scan0);
                image.UnlockBits(imageData);
            }
        }
        #endregion
    }
}