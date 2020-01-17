using Microsoft.Win32;
using SharpGL.SceneGraph;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                //m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Duck"), "duck.dae", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\new_penguin"), "pengiun.obj", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.Width, (int)openGLControl.Height);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F4: this.Close(); break;
                case Key.D:
                    if (m_world.RotationX > 0.0f) {
                        m_world.RotationX -= 5.0f;
                    }
                    break;
                case Key.E:
                    if (m_world.RotationX < 90.0f) {
                        m_world.RotationX += 5.0f;
                    }
                    break;
                case Key.F: m_world.RotationY -= 5.0f; break;
                case Key.S: m_world.RotationY += 5.0f; break;
                case Key.Add:  
                    if (m_world.SceneDistance > 30.0f)
                    {
                        m_world.SceneDistance -= 5.0f;
                    }
                    break;
                case Key.Subtract: 
                    if (m_world.SceneDistance < 100.0f)
                    {
                        m_world.SceneDistance += 5.0f;
                    }
                    break;
                case Key.F2:
                    OpenFileDialog opfModel = new OpenFileDialog();
                    bool result = (bool) opfModel.ShowDialog();
                    if (result)
                    {

                        try
                        {
                            World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                            m_world.Dispose();
                            m_world = newWorld;
                            m_world.Initialize(openGLControl.OpenGL);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK );
                        }
                    }
                    break;
                case Key.V:
                    m_world.Animacija = true;
                    m_world.I = 0;
                    break;
                case Key.B:
                    m_world.I = 5;
                    break;
            }
        }

        private void RotateIglooLeft(object sender, RoutedEventArgs e)
        {
            if (m_world.IglooRY == -90f) //desno
            {
                //napred
                m_world.IglooTX = m_world.IglooNapredTrans[0];
                m_world.IglooTY = m_world.IglooNapredTrans[1];
                m_world.IglooTZ = m_world.IglooNapredTrans[2];

                m_world.IglooRX = m_world.IglooNapredRot[0];
                m_world.IglooRY = m_world.IglooNapredRot[1];
                m_world.IglooRZ = m_world.IglooNapredRot[2];
            }
            else if (m_world.IglooRY == 180f) //napred
            {
                //levo
                m_world.IglooTX = m_world.IglooLevoTrans[0];
                m_world.IglooTY = m_world.IglooLevoTrans[1];
                m_world.IglooTZ = m_world.IglooLevoTrans[2];

                m_world.IglooRX = m_world.IglooLevoRot[0];
                m_world.IglooRY = m_world.IglooLevoRot[1];
                m_world.IglooRZ = m_world.IglooLevoRot[2];
            }
            else if (m_world.IglooRY == 90f) //levo
            {
                //nazad
                m_world.IglooTX = m_world.IglooNazadTrans[0];
                m_world.IglooTY = m_world.IglooNazadTrans[1];
                m_world.IglooTZ = m_world.IglooNazadTrans[2];

                m_world.IglooRX = m_world.IglooNazadRot[0];
                m_world.IglooRY = m_world.IglooNazadRot[1];
                m_world.IglooRZ = m_world.IglooNazadRot[2];
            }
            else if (m_world.IglooRY == 0f) // nazad
            {
                //desno
                m_world.IglooTX = m_world.IglooDesnoTrans[0];
                m_world.IglooTY = m_world.IglooDesnoTrans[1];
                m_world.IglooTZ = m_world.IglooDesnoTrans[2];

                m_world.IglooRX = m_world.IglooDesnoRot[0];
                m_world.IglooRY = m_world.IglooDesnoRot[1];
                m_world.IglooRZ = m_world.IglooDesnoRot[2];
            }
        }

        private void RotateIglooRight(object sender, RoutedEventArgs e)
        {
            if (m_world.IglooRY == -90f) //desno
            {
                //nazad
                m_world.IglooTX = m_world.IglooNazadTrans[0];
                m_world.IglooTY = m_world.IglooNazadTrans[1];
                m_world.IglooTZ = m_world.IglooNazadTrans[2];

                m_world.IglooRX = m_world.IglooNazadRot[0];
                m_world.IglooRY = m_world.IglooNazadRot[1];
                m_world.IglooRZ = m_world.IglooNazadRot[2];
            }
            else if (m_world.IglooRY == 180f) //napred
            {
                //desno
                m_world.IglooTX = m_world.IglooDesnoTrans[0];
                m_world.IglooTY = m_world.IglooDesnoTrans[1];
                m_world.IglooTZ = m_world.IglooDesnoTrans[2];

                m_world.IglooRX = m_world.IglooDesnoRot[0];
                m_world.IglooRY = m_world.IglooDesnoRot[1];
                m_world.IglooRZ = m_world.IglooDesnoRot[2];
            }
            else if (m_world.IglooRY == 90f) //levo
            {
                //napred
                m_world.IglooTX = m_world.IglooNapredTrans[0];
                m_world.IglooTY = m_world.IglooNapredTrans[1];
                m_world.IglooTZ = m_world.IglooNapredTrans[2];

                m_world.IglooRX = m_world.IglooNapredRot[0];
                m_world.IglooRY = m_world.IglooNapredRot[1];
                m_world.IglooRZ = m_world.IglooNapredRot[2];
            }
            else if (m_world.IglooRY == 0f) // nazad
            {
                //levo
                m_world.IglooTX = m_world.IglooLevoTrans[0];
                m_world.IglooTY = m_world.IglooLevoTrans[1];
                m_world.IglooTZ = m_world.IglooLevoTrans[2];

                m_world.IglooRX = m_world.IglooLevoRot[0];
                m_world.IglooRY = m_world.IglooLevoRot[1];
                m_world.IglooRZ = m_world.IglooLevoRot[2];
            }
        }
    }
}
