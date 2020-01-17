// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace AssimpSample
{
    public enum TextureFilterMode
    {
        Nearest,
        Linear,
        NearestMipmapNearest,
        NearestMipmapLinear,
        LinearMipmapNearest,
        LinearMipmapLinear
    };


    public class World : IDisposable
    {
        #region Atributi

        #region Teksture
        private enum TextureObjects { Sea = 0, Ice, Snow};
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

        private uint[] m_textures = null;

        //private string[] m_textureFiles = { "..//..//images//sea.jpg", "..//..//images//ice.jpg", "..//..//images//snow.jpg" };
        private static string sea = "E://7. Semestar//Racunarska grafika//Vezbe//RG - RA vezbe 4//RG-E2 Vezbe 4//AssimpSample//AssimpSample//images//water-sea.jpg";
        private static string ice = "E://7. Semestar//Racunarska grafika//Vezbe//RG - RA vezbe 4//RG-E2 Vezbe 4//AssimpSample//AssimpSample//images//ice.jpg";
        private static string snow = "E://7. Semestar//Racunarska grafika//Vezbe//RG - RA vezbe 4//RG-E2 Vezbe 4//AssimpSample//AssimpSample//images//snow.jpg";
        private string[] m_textureFiles = { sea, ice, snow };

        private TextureFilterMode m_selectedMode = TextureFilterMode.Linear;
        #endregion
        #region Scena
        private AssimpScene m_scene;

        private float m_xRotation = 0.0f;

        private float m_yRotation = 0.0f;

        private float m_sceneDistance = 65.0f;

        private int m_width;

        private int m_height;
        #endregion
        #region Iglo
        private float igloo_rotate = -90.0f;

        private float iglooTX = -16f;
        private float iglooTY = -5f;
        private float iglooTZ = -15f;

        private float iglooRX = -90f;
        private float iglooRY = -90f;
        private float iglooRZ = 0f;
        #endregion
        #region Animacija

        private int i=0;
        private bool animacija;
        private float pengRotationX;
        private float pengRotationY;
        private float pengRotationZ;
        private float pengTransX;
        private float pengTransY;
        private float pengTransZ;

        #endregion

        private OpenGL gl;

        #endregion Atributi

        #region Properties

        #region Teksture
        public TextureFilterMode SelectedMode
        {
            get { return m_selectedMode; }
            set
            {
                m_selectedMode = value;

                foreach (uint textureId in m_textures)
                {
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);

                    switch (m_selectedMode)
                    {
                        case TextureFilterMode.Nearest:
                            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
                            break;

                        case TextureFilterMode.Linear:
                            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
                            break;

                        case TextureFilterMode.NearestMipmapNearest:
                            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST_MIPMAP_NEAREST);
                            break;

                        case TextureFilterMode.NearestMipmapLinear:
                            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST_MIPMAP_LINEAR);
                            break;

                        case TextureFilterMode.LinearMipmapNearest:
                            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_NEAREST);
                            break;

                        case TextureFilterMode.LinearMipmapLinear:
                            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
                            //gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
                            break;
                    }
                }
            }
        }
        #endregion
        #region Scena
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }
        #endregion
        #region Iglo
        public float IglooRotate
        {
            get { return igloo_rotate; }
            set { igloo_rotate = value; }
        }

        public float[] IglooLevoTrans = { -52f, -5f, -15f };
        public float[] IglooLevoRot = { 0f, 90f, 0f };

        public float[] IglooNapredTrans = { -35f, -5f, 4f };
        public float[] IglooNapredRot = { 0f, 180f, 0f };

        public float[] IglooDesnoTrans = { -16f, -5f, -15f };
        public float[] IglooDesnoRot = { -90f, -90f, 0f };

        public float[] IglooNazadTrans = { -35f, -5f, -34f };
        public float[] IglooNazadRot = { 0f, 0f, 0f };

        public float IglooTX
        {
            get { return iglooTX; }
            set { iglooTX = value; }
        }
        public float IglooTY
        {
            get { return iglooTY; }
            set { iglooTY = value; }
        }
        public float IglooTZ
        {
            get { return iglooTZ; }
            set { iglooTZ = value; }
        }

        public float IglooRX
        {
            get { return iglooRX; }
            set { iglooRX = value; }
        }
        public float IglooRY
        {
            get { return iglooRY; }
            set { iglooRY = value; }
        }
        public float IglooRZ
        {
            get { return iglooRZ; }
            set { iglooRZ = value; }
        }
        #endregion
        #region Animacija
        public bool Animacija { get => animacija; set => animacija = value; }
        public int I { get => i; set => i = value; }
        #endregion

        #endregion Properties

        #region Konstruktori

        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
            this.gl = gl;
            m_textures = new uint[m_textureCount];
        }

        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.7f, 0.9f, 1.0f, 0.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Enable(OpenGL.GL_NORMALIZE);

            SunLight(gl);
            RedLight(gl);
            Textures(gl);

            m_scene.LoadScene();
            m_scene.Initialize();
        }

        #region Svetlo
        private void SunLight(OpenGL gl)
        {
            float[] global_ambient = new float[] { 0.3f, 0.3f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            float[] light0pos = new float[] { 2.0f, 1.70f, 0.0f, 0.0f };
            float[] light0ambient = new float[] { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 8.8f, 8.8f, 0.8f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f); // podrazumevani ugao (tackasti izvor)
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, light0specular);
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 100.0f);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }

        private void RedLight(OpenGL gl)
        {
            float[] ambijentalnaKomponenta = { 0.7f, 0.3f, 0.3f, 1.0f };
            float[] difuznaKomponenta = { 0.7f, 0.1f, 0.1f, 1.0f };
            float[] smer = { 0.0f, -1.0f, .0f };
            // Pridruži komponente svetlosnom izvoru 0
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, ambijentalnaKomponenta);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, difuznaKomponenta);
            // Podesi parametre reflektorkskog izvora
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            // Ukljuci svetlosni izvor
            gl.Enable(OpenGL.GL_LIGHT1);
            // Pozicioniraj svetlosni izvor
            float[] pozicija = { 1.0f, 5.0f, 10.0f, 0.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, pozicija);
        }
        
        #endregion
        #region Teksture
        public void Textures(OpenGL gl)
        {
            // Teksture se primenjuju sa parametrom modulate
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            // Ucitaj slike i kreiraj teksture
            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                if (i < 2)
                {
                    gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
                }
                else
                {
                    gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
                }
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                Bitmap image = new Bitmap(m_textureFiles[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST_MIPMAP_NEAREST);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST_MIPMAP_NEAREST);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                image.UnlockBits(imageData);
                image.Dispose();
            }
        }
        #endregion
        #region Objekti
        public void PenguinDrawing(OpenGL gl)
        {
            gl.Translate(pengTransX, pengTransY, pengTransZ);
            gl.Rotate(pengRotationX, pengRotationY, 0.0f);
            m_scene.Draw();
        }

        public void WaterDrawing(OpenGL gl)
        {
            Grid grid = new Grid();
            gl.Translate(-10f, -5.4f, 0f);
            //gl.Scale(2, 2, 2);
            gl.DrawText3D("Arial", 25f, 1f, 0.1f, "");
            gl.Rotate(-90f, 0f, 90f);
            //grid.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);

            gl.Color(0.196f, 0.6f, 0.8f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Sea]);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-420.0f, -420.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(420.0f, -420.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(420.0f, 420.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-420.0f, 420.0f);    
            gl.End();

        }

        public void IceDrawing(OpenGL gl)
        {
            gl.Translate(-10f, -5.2f, 8f);
            gl.Rotate(-90f, 0f, 90f);
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Ice]);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-40.0f, -40.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(0.0f, -40.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(0.0f, 0.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-40.0f, 0.0f);
            gl.End();

            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-25.0f, -30.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(40.0f, -30.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(50.0f, 55.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-25.0f, 55.0f);
            gl.End();
        }

        public void IglooSphere(OpenGL gl)
        {
            gl.Color(0.8f, 0.7f, 1.0f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Snow]);
            gl.Translate(-35.0f, -5.0f, 0.0f);
            gl.Scale(17, 17, 17);
            Sphere sphere = new Sphere();
            sphere.TextureCoords = true;
            sphere.CreateInContext(gl);
            sphere.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Design);
        }

        public void IglooCylinder(OpenGL gl)
        {
            gl.Translate(iglooTX, iglooTY, iglooTZ+15.0f);
            gl.Rotate(iglooRX, iglooRY, iglooRZ);
            gl.Scale(14, 14, 32);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Snow]);
            Cylinder cil = new Cylinder();
            cil.TextureCoords = true;
            cil.CreateInContext(gl);
            cil.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
        }
        #endregion
        #region Tekst
        public void TextDrawing(OpenGL gl)
        {
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.DrawText(620, 100, 1.0f, 0.0f, 0.0f, "Verdana", 10, "Predmet: Racunarska grafika");
            gl.DrawText(620, 85, 1.0f, 0.0f, 0.0f, "Verdana", 10, "Sk. god: 2019/20");
            gl.DrawText(620, 70, 1.0f, 0.0f, 0.0f, "Verdana", 10, "Ime: Jelena");
            gl.DrawText(620, 55, 1.0f, 0.0f, 0.0f, "Verdana", 10, "Prezime: Dostic");
            gl.DrawText(620, 40, 1.0f, 0.0f, 0.0f, "Verdana", 10, "Sifra zad: 10.2");
        }
        #endregion
        #region Animacija
        public void PokreniAnimaciju()
        {
            if (i == 0)
            {
                pengTransX = 27f;
                pengTransY = -20f;
                i = 1;
            }
            pengRotationY = -90.0f;

            if (i == 1)
            {
                pengTransY += 5f;
                pengTransX -= 0.7f;

                if (pengTransY == 10f)
                {
                    i = 2;
                }
            }

            if (i == 2)
            {
                pengTransY -= 5f;
                pengTransX -= 1.8f;

                if (pengTransY == 0f)
                {
                    i = 3;
                }
            }

            if (i == 3)
            {
                pengTransX -= 2f;
                if (pengRotationY == -90f)
                {
                    pengRotationY -= 5f;
                }
                else if (pengRotationY < -90f)
                {
                    pengRotationY += 10f;
                }
                else if (pengRotationY > -90f)
                {
                    pengRotationY -= 10f;
                }

                if (pengTransX <= -30f)
                {
                    i = 4;
                }
            }

            if (i == 5)
            {
                pengRotationY = 0f;
                pengTransX = 0f;
            }
        }

        #endregion

        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(m_sceneDistance, m_width / (double)m_height, 0.5, 100.0);
            // gl.LookAt(0f, 0f, -SceneDistance - 40, 0f, 0f, -SceneDistance, 0f, 0f, -1f);
            gl.Ortho(100f, 100f, 1f, 100f, 0.5, 100.0);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PushMatrix();

            gl.Translate(4.0f, -2.0f, -55.0f);
            gl.Scale(0.5, 0.5, 0.5);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            gl.PushMatrix();
            PenguinDrawing(gl);  
            gl.PopMatrix();

            gl.PushMatrix();
            IglooSphere(gl);
            gl.PopMatrix();

            gl.PushMatrix();
            IglooCylinder(gl);
            gl.PopMatrix();

            gl.PushMatrix();
            WaterDrawing(gl);
            gl.PopMatrix();

            gl.PushMatrix();
            IceDrawing(gl);
            gl.PopMatrix();

            gl.PushMatrix();
            TextDrawing(gl);
            gl.PopMatrix();

            gl.PopMatrix();

            if (Animacija)
            {
                PokreniAnimaciju();
            }

            gl.Flush();

        }

        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Viewport(0, 0, m_width, m_height);
            gl.Perspective(70f, (double)width / height, 1.0f, 200f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
