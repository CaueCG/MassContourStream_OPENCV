using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Point = OpenCvSharp.Point;

namespace Z_LEARNING_OPENCV
{
    public partial class Form1 : Form
    {
        private VideoCapture videoCapture;
        private Mat frame;
        private Mat frameProcessado;

        private Point[] pontosPoligono;
        private bool desenhandoPoligono;

        private BackgroundSubtractorMOG2 backgroundSubtractor;

        public Form1()
        {
            InitializeComponent();

            pictureBox1.MouseClick += PictureBox1_MouseClick;
            pictureBox1.Paint += PictureBox1_Paint;

            desenhandoPoligono = true;
            pontosPoligono = new Point[0];

            InicializarCamera();
        }

        private void InicializarCamera()
        {
            videoCapture = new VideoCapture();
            videoCapture.Open("rtsp://admin:imp12345@192.168.1.109:554/stream?channel=1");

            if (!videoCapture.IsOpened())
            {
                MessageBox.Show("Não foi possível abrir a câmera.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            frame = new Mat();
            frameProcessado = new Mat();

            // Inicializar o modelo de subtração de fundo
            backgroundSubtractor = BackgroundSubtractorMOG2.Create();

            Application.Idle += ProcessFrames;
        }

        private void ProcessFrames(object sender, EventArgs e)
        {
            videoCapture.Read(frame);

            if (!frame.Empty())
            {
                Cv2.Resize(frame, frame, new OpenCvSharp.Size(640, 480));

                if (pontosPoligono.Length > 3)
                {            
                    // Converte o frame para escala de cinza para facilitar a detecção de contornos
                    Cv2.CvtColor(frame, frameProcessado, ColorConversionCodes.BGR2GRAY);

                    // Realiza a limiarização para binarizar a imagem]
                    Cv2.Threshold(frameProcessado, frameProcessado, 127, 255, ThresholdTypes.Binary);

                    // Aplicar subtração de fundo
                    using (var mask = new Mat(frameProcessado.Size(), MatType.CV_8UC1, Scalar.All(0)))
                    {
                        Cv2.FillPoly(mask, new[] { pontosPoligono }, Scalar.All(255));
                        Cv2.BitwiseAnd(frameProcessado, mask, frameProcessado);
                    }

                    Mat maskRemoveBackground = new Mat();
                    backgroundSubtractor.Apply(frameProcessado, maskRemoveBackground);

                    // Encontrar contornos na máscara resultante
                    Point[][] contornos;
                    HierarchyIndex[] hierarchy;
                    Cv2.FindContours(maskRemoveBackground, out contornos, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

                    // Filtrar contornos pré-existentes
                    contornos = FiltrarContornosPreexistentes(contornos);

                    // Desenhar contornos na imagem original
                    for (int i = 0; i < contornos.Length; i++)
                        Cv2.DrawContours(frame, contornos, i, Scalar.Red, 2);
                }

                AtualizarPictureBox();
            }
        }

        private Point[][] FiltrarContornosPreexistentes(Point[][] contornos)
        {
            List<Point[]> contornosFiltrados = new List<Point[]>();

            double areaMinima = 100.0;

            foreach (var contorno in contornos)
            {
                double area = Cv2.ContourArea(contorno);

                if (area > areaMinima)
                    contornosFiltrados.Add(contorno);
            }

            return contornosFiltrados.ToArray();
        }

        private void AtualizarPictureBox()
        {
            pictureBox1.Image = frame.ToBitmap();
        }

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (desenhandoPoligono)
            {
                // Adiciona pontos ao polígono
                Array.Resize(ref pontosPoligono, pontosPoligono.Length + 1);
                pontosPoligono[pontosPoligono.Length - 1] = new Point(e.X, e.Y);

                // Redesenha o polígono
                pictureBox1.Invalidate();
            }
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (desenhandoPoligono)
            {
                // Desenha o polígono no PictureBox
                if (pontosPoligono.Length > 1)
                {
                    var pontosPointF = Array.ConvertAll(pontosPoligono, p => new PointF(p.X, p.Y));
                    e.Graphics.DrawPolygon(Pens.Blue, pontosPointF);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
