using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WoodPixels
{
    public partial class Form1 : Form
    {
        Image<Bgr, byte> Image_Target;
        Image<Bgr, byte> Image_Texture;
        Image<Bgr, byte> Image_Result;
        String path = @"C:\Users\ivam\Desktop\WoodPixels_files";
        int size = 16;
        int rotations = 16;
        double scale = 1.0;
        double weight_hist = 0.5;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            trackBar_size_Scroll(null, null);
            trackBar_Rotations_Scroll(null, null);
            trackBar_scale_Scroll(null, null);
            trackBar_hist_Scroll(null, null);
        }
        private void button_StartMatching_Click(object sender, EventArgs e)
        {
            if (Image_Target == null || Image_Texture == null)
            {
                return;
            }
            size = trackBar_size.Value;
            rotations = trackBar_Rotations.Value;
            scale = (double)trackBar_scale.Value / 10.0;
            weight_hist = (double)trackBar_hist.Value / 10.0;
            Thread t = new Thread(() =>
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    groupBox_Parameters.Enabled = false;
                    button_StartMatching.Enabled = false;
                }));
                //GeneratePatches();
                //StartMatching();
                StartMatchingSaliency();
                MergePatches();
                this.Invoke(new MethodInvoker(() =>
                {
                    groupBox_Parameters.Enabled = true;
                    button_StartMatching.Enabled = true;
                }));
            });

            t.IsBackground = true;
            t.Start();
        }
        private void StartMatching()
        {
            Image<Bgr, Byte> target = Image_Target.Clone().Resize(scale, Inter.Linear);
            Image<Bgr, Byte> texture = Image_Texture.Clone();
            Image<Gray, Byte> target_gray = target.Convert<Gray, Byte>();
            Image<Gray, Byte> texture_gray = texture.Convert<Gray, Byte>();
            Image<Gray, Byte> target_hist_matched = new Image<Gray, Byte>(target.Size);
            Image<Gray, Byte> target_hist_matched_weighted = new Image<Gray, Byte>(target.Size);
            Image<Gray, float> target_sobel_x = target_gray.Sobel(1, 0, 3);
            Image<Gray, float> target_sobel_y = target_gray.Sobel(0, 1, 3);
            Image<Gray, float> texture_sobel_x = texture_gray.Sobel(1, 0, 3);
            Image<Gray, float> texture_sobel_y = texture_gray.Sobel(0, 1, 3);
            Image<Gray, float> target_sobel_mag = new Image<Gray, float>(target_gray.Size);
            Image<Gray, float> texture_sobel_mag = new Image<Gray, float>(texture_gray.Size);
            Image_Result = new Image<Bgr, byte>(target.Width, target.Height, new Bgr(0, 0, 0));
            imageBox_Result.Image = Image_Result;

            this.Invoke(new MethodInvoker(() =>
            {
                progressBar_match.Value = 0;
                progressBar_match.Maximum = (target.Width / size) * (target.Height / size);
            }));

            Matrix<byte> histLUT = new Matrix<byte>(1, 256);
            Mat hist_target = new Mat();
            Mat hist_texture = new Mat();

            VectorOfMat vm_target = new VectorOfMat();
            VectorOfMat vm_texture = new VectorOfMat();
            vm_target.Push(target_gray);
            vm_texture.Push(texture_gray);

            CvInvoke.CalcHist(vm_target, new int[] { 0 }, null, hist_target, new int[] { 256 }, new float[] { 0, 255 }, false);
            CvInvoke.CalcHist(vm_texture, new int[] { 0 }, null, hist_texture, new int[] { 256 }, new float[] { 0, 255 }, false);

            float[] CDF_hist_target = new float[256];
            float[] CDF_hist_texture = new float[256];
            Marshal.Copy(hist_target.DataPointer, CDF_hist_target, 0, 256);
            Marshal.Copy(hist_texture.DataPointer, CDF_hist_texture, 0, 256);

            for (int i = 1; i < 256; i++)
            {
                CDF_hist_target[i] += CDF_hist_target[i - 1];
                CDF_hist_texture[i] += CDF_hist_texture[i - 1];
            }

            for (int i = 0; i < 256; i++)
            {
                histLUT.Data[0, i] = 0;
                for (int j = 0; j < 256; j++)
                {
                    if (CDF_hist_texture[j] >= CDF_hist_target[i])
                    {
                        histLUT.Data[0, i] = (byte)j;
                        break;
                    }
                }
            }
            CvInvoke.LUT(target_gray, histLUT, target_hist_matched);
            target_hist_matched_weighted = target_hist_matched * weight_hist + target_gray * (1.0 - weight_hist);

            CvInvoke.CartToPolar(target_sobel_x, target_sobel_y, target_sobel_mag, new Mat());
            CvInvoke.CartToPolar(texture_sobel_x, texture_sobel_y, texture_sobel_mag, new Mat());


            List<Matrix<float>> transformation_matrixs = new List<Matrix<float>>();
            List<Matrix<float>> transformation_matrixs_invert = new List<Matrix<float>>();
            List<RectangleF> rotatedRects = new List<RectangleF>();

            for (int i = 1; i < rotations; i++)
            {
                double angle = i * (360.0 / (float)rotations);
                RectangleF rotatedRect = new RotatedRect(new PointF(), texture.Size, (float)angle).MinAreaRect();
                PointF center = new PointF(0.5f * texture.Width, 0.5f * texture.Height);
                Matrix<float> transformation_matrix = new Matrix<float>(2, 3);
                Matrix<float> transformation_matrix_invert = new Matrix<float>(2, 3);
                CvInvoke.GetRotationMatrix2D(center, angle, 1.0, transformation_matrix);

                transformation_matrix.Data[0, 2] += (rotatedRect.Width - texture.Width) / 2;
                transformation_matrix.Data[1, 2] += (rotatedRect.Height - texture.Height) / 2;
                CvInvoke.InvertAffineTransform(transformation_matrix, transformation_matrix_invert);
                transformation_matrixs.Add(transformation_matrix);
                transformation_matrixs_invert.Add(transformation_matrix_invert);
                rotatedRects.Add(rotatedRect);
            }

            List<Image<Bgr, byte>> texture_rotations = new List<Image<Bgr, byte>>(rotations) { };
            List<Image<Gray, byte>> texture_gray_rotations = new List<Image<Gray, byte>>(rotations) { };
            List<Image<Gray, float>> texture_sobel_rotations = new List<Image<Gray, float>>(rotations) { };
            List<Image<Gray, byte>> texture_mask_rotations = new List<Image<Gray, byte>>(rotations) { };

            texture_rotations.Add(texture);
            texture_gray_rotations.Add(texture_gray);
            texture_sobel_rotations.Add(texture_sobel_mag);
            texture_mask_rotations.Add(new Image<Gray, byte>(texture.Width, texture.Height, new Gray(255)));
            for (int i = 1; i < rotations; i++)
            {
                texture_mask_rotations.Add(new Image<Gray, byte>(rotatedRects[i - 1].Size.ToSize()));
                texture_rotations.Add(new Image<Bgr, byte>(rotatedRects[i - 1].Size.ToSize()));
                texture_gray_rotations.Add(new Image<Gray, byte>(rotatedRects[i - 1].Size.ToSize()));
                texture_sobel_rotations.Add(new Image<Gray, float>(rotatedRects[i - 1].Size.ToSize()));
            }
            for (int i = 1; i < rotations; i++)
            {
                CvInvoke.WarpAffine(texture, texture_rotations[i], transformation_matrixs[i - 1], rotatedRects[i - 1].Size.ToSize(), Inter.Nearest, Warp.Default, BorderType.Constant, new MCvScalar(0));
                CvInvoke.WarpAffine(texture_gray, texture_gray_rotations[i], transformation_matrixs[i - 1], rotatedRects[i - 1].Size.ToSize(), Inter.Nearest, Warp.Default, BorderType.Constant, new MCvScalar(0));
                CvInvoke.WarpAffine(texture_sobel_mag, texture_sobel_rotations[i], transformation_matrixs[i - 1], rotatedRects[i - 1].Size.ToSize(), Inter.Nearest, Warp.Default, BorderType.Constant, new MCvScalar(0));
                CvInvoke.WarpAffine(texture_mask_rotations[0], texture_mask_rotations[i], transformation_matrixs[i - 1], rotatedRects[i - 1].Size.ToSize(), Inter.Nearest, Warp.Default, BorderType.Constant, new MCvScalar(0));
            }

            // Directory.SetCurrentDirectory(path);
            String current_path = path + @"\matched_patches";
            if (Directory.Exists(current_path))
            {
                DirectoryInfo di = new DirectoryInfo(current_path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            else
            {
                DirectoryInfo di = Directory.CreateDirectory(current_path);
            }

            for (int y = 0; y < target_hist_matched_weighted.Height / size; y++)
            {
                for (int x = 0; x < target_hist_matched_weighted.Width / size; x++)
                {
                    Image<Bgr, byte> template;
                    Image<Gray, byte> template_gray;
                    Image<Gray, float> template_sobel;

                    template = target.Clone();
                    template_gray = target_hist_matched_weighted.Clone();
                    template_sobel = target_sobel_mag.Clone();
                    template.ROI = new Rectangle(x * size, y * size, size, size);
                    template_gray.ROI = new Rectangle(x * size, y * size, size, size);
                    template_sobel.ROI = new Rectangle(x * size, y * size, size, size);
                    template = template.Clone();
                    template_gray = template_gray.Copy();
                    template_sobel = template_sobel.Copy();

                    int minMatchIndex = -1;
                    double minMatchValue = double.MaxValue;
                    Point minMatchLoc = new Point();
                    Object _lock = new Object();
                    Parallel.For(0, rotations, i =>
                    {
                        Image<Gray, float> match_gray = new Image<Gray, float>(texture_gray.Size);
                        Image<Gray, float> match_sobel = new Image<Gray, float>(texture_sobel_mag.Size);
                        Image<Gray, float> match_sum = new Image<Gray, float>(texture.Size);
                        double minVal = 0, maxVal = 0;
                        Point minLoc = new Point(), maxLoc = new Point();

                        CvInvoke.MatchTemplate(texture_gray_rotations[i], template_gray, match_gray, TemplateMatchingType.Sqdiff);
                        CvInvoke.MatchTemplate(texture_sobel_rotations[i], template_sobel, match_sobel, TemplateMatchingType.Sqdiff);
                        match_sum = match_gray + match_sobel;
                        //CvInvoke.MinMaxLoc(match_sum, ref minVal, ref maxVal, ref minLoc, ref maxLoc);
                        //CudaInvoke.MinMaxLoc(match_sum, ref minVal, ref maxVal, ref minLoc, ref maxLoc, GetMinMaxLocMask(texture_mask_rotations[i], size));
                        CvInvoke.MinMaxLoc(match_sum, ref minVal, ref maxVal, ref minLoc, ref maxLoc, GetMinMaxLocMask(texture_mask_rotations[i], size));
                        lock (_lock)
                        {
                            if (minVal < minMatchValue && minVal > 0)
                            {
                                minMatchValue = minVal;
                                minMatchIndex = i;
                                minMatchLoc = minLoc;
                            }
                        }
                        match_gray.Dispose();
                        match_sobel.Dispose();
                        match_sum.Dispose();
                    });
                    Console.WriteLine($"minMatchValue = {minMatchValue}\r\nminMatchIndex = {minMatchIndex}\r\nminMatchLoc = {minMatchLoc}");
                    if (minMatchIndex < 0)
                    {
                        MessageBox.Show("Out of textures!!!!");
                        return;
                    }
                    texture_mask_rotations[minMatchIndex].Draw(new Rectangle(minMatchLoc.X, minMatchLoc.Y, size - 1, size - 1), new Gray(0), -1);
                    if (minMatchIndex > 0)
                    {
                        Image<Gray, byte> mask = new Image<Gray, byte>(texture_mask_rotations[0].Size);
                        CvInvoke.WarpAffine(texture_mask_rotations[minMatchIndex], mask, transformation_matrixs_invert[minMatchIndex - 1], mask.Size, Inter.Nearest, Warp.Default, BorderType.Constant, new MCvScalar(0));
                        texture_mask_rotations[0] = texture_mask_rotations[0] - (255 - mask);
                    }
                    for (int i = 1; i < rotations; i++)
                    {
                        Image<Gray, byte> mask_rot = new Image<Gray, byte>(texture_mask_rotations[i].Size);
                        CvInvoke.WarpAffine(texture_mask_rotations[0], mask_rot, transformation_matrixs[i - 1], mask_rot.Size, Inter.Nearest, Warp.Default, BorderType.Constant, new MCvScalar(0));
                        mask_rot.CopyTo(texture_mask_rotations[i]);
                    }

                    texture_rotations[minMatchIndex].ROI = new Rectangle(minMatchLoc, new Size(size, size));
                    CvInvoke.Imwrite($@"{current_path}\{x}_{y}.bmp", texture_rotations[minMatchIndex].Copy());
                    imageBox_match.Image = texture_rotations[minMatchIndex].Copy();
                    imageBox_template.Image = template.Copy();
                    Image_Result.ROI = new Rectangle(x * size, y * size, size, size);
                    texture_rotations[minMatchIndex].CopyTo(Image_Result);

                    Image_Result.ROI = Rectangle.Empty;
                    texture_rotations[minMatchIndex].ROI = Rectangle.Empty;

                    imageBox_Result.Image = Image_Result;


                    if (minMatchIndex > 0)
                    {
                        PointF[] maskPoints = { minMatchLoc, new PointF(minMatchLoc.X + size - 1, minMatchLoc.Y), new PointF(minMatchLoc.X, minMatchLoc.Y + size - 1), new PointF(minMatchLoc.X + size - 1, minMatchLoc.Y + size - 1) };
                        PointF[] maskPoints_rot = new PointF[4];
                        for (int i = 0; i < 4; i++)
                        {
                            maskPoints_rot[i].X = maskPoints[i].X * transformation_matrixs_invert[minMatchIndex - 1].Data[0, 0] + maskPoints[i].Y * transformation_matrixs_invert[minMatchIndex - 1].Data[0, 1] + 1.0f * transformation_matrixs_invert[minMatchIndex - 1].Data[0, 2];
                            maskPoints_rot[i].Y = maskPoints[i].X * transformation_matrixs_invert[minMatchIndex - 1].Data[1, 0] + maskPoints[i].Y * transformation_matrixs_invert[minMatchIndex - 1].Data[1, 1] + 1.0f * transformation_matrixs_invert[minMatchIndex - 1].Data[1, 2];
                        }
                        Point[] maskPoints_rot_round = { Point.Round(maskPoints_rot[0]), Point.Round(maskPoints_rot[1]), Point.Round(maskPoints_rot[3]), Point.Round(maskPoints_rot[2]) };
                        texture.FillConvexPoly(maskPoints_rot_round, new Bgr(0, 0, 0));
                    }
                    else
                    {
                        //texture.DrawPolyline(new Point[] { minMatchLoc, new Point(minMatchLoc.X + size, minMatchLoc.Y), new Point(minMatchLoc.X + size, minMatchLoc.Y + size), new Point(minMatchLoc.X, minMatchLoc.Y + size) }, true, new Bgr(0, 0, 0), 0);
                        texture.Draw(new Rectangle(minMatchLoc, new Size(size - 1, size - 1)), new Bgr(0, 0, 0), -1);
                    }
                    imageBox_Texture.Image = texture;

                    template.Dispose();
                    template_gray.Dispose();
                    template_sobel.Dispose();
                    GC.Collect();

                    this.Invoke(new MethodInvoker(() =>
                    {
                        progressBar_match.Value++;
                    }));

                }
            }
        }

        private void StartMatchingSaliency()
        {
            Image<Bgr, Byte> target = Image_Target.Clone().Resize(scale, Inter.Linear);
            Image<Bgr, Byte> texture = Image_Texture.Clone();
            Image<Gray, Byte> target_gray = target.Convert<Gray, Byte>();
            Image<Gray, Byte> texture_gray = texture.Convert<Gray, Byte>();
            Image<Gray, Byte> target_hist_matched = new Image<Gray, Byte>(target.Size);
            Image<Gray, Byte> target_hist_matched_weighted = new Image<Gray, Byte>(target.Size);
            Image<Gray, float> target_sobel_x = target_gray.Sobel(1, 0, 3);
            Image<Gray, float> target_sobel_y = target_gray.Sobel(0, 1, 3);
            Image<Gray, float> texture_sobel_x = texture_gray.Sobel(1, 0, 3);
            Image<Gray, float> texture_sobel_y = texture_gray.Sobel(0, 1, 3);
            Image<Gray, float> target_sobel_mag = new Image<Gray, float>(target_gray.Size);
            Image<Gray, float> texture_sobel_mag = new Image<Gray, float>(texture_gray.Size);
            Image_Result = new Image<Bgr, byte>(target.Width, target.Height, new Bgr(0, 0, 0));
            imageBox_Result.Image = Image_Result;

            this.Invoke(new MethodInvoker(() =>
            {
                progressBar_match.Value = 0;
                progressBar_match.Maximum = (target.Width / size) * (target.Height / size);
            }));

            Matrix<byte> histLUT = new Matrix<byte>(1, 256);
            Mat hist_target = new Mat();
            Mat hist_texture = new Mat();

            VectorOfMat vm_target = new VectorOfMat();
            VectorOfMat vm_texture = new VectorOfMat();
            vm_target.Push(target_gray);
            vm_texture.Push(texture_gray);

            CvInvoke.CalcHist(vm_target, new int[] { 0 }, null, hist_target, new int[] { 256 }, new float[] { 0, 255 }, false);
            CvInvoke.CalcHist(vm_texture, new int[] { 0 }, null, hist_texture, new int[] { 256 }, new float[] { 0, 255 }, false);

            float[] CDF_hist_target = new float[256];
            float[] CDF_hist_texture = new float[256];
            Marshal.Copy(hist_target.DataPointer, CDF_hist_target, 0, 256);
            Marshal.Copy(hist_texture.DataPointer, CDF_hist_texture, 0, 256);

            for (int i = 1; i < 256; i++)
            {
                CDF_hist_target[i] += CDF_hist_target[i - 1];
                CDF_hist_texture[i] += CDF_hist_texture[i - 1];
            }

            for (int i = 0; i < 256; i++)
            {
                histLUT.Data[0, i] = 0;
                for (int j = 0; j < 256; j++)
                {
                    if (CDF_hist_texture[j] >= CDF_hist_target[i])
                    {
                        histLUT.Data[0, i] = (byte)j;
                        break;
                    }
                }
            }
            CvInvoke.LUT(target_gray, histLUT, target_hist_matched);
            target_hist_matched_weighted = target_hist_matched * weight_hist + target_gray * (1.0 - weight_hist);
            imageBox_hist.Image = target_hist_matched_weighted;

            CvInvoke.CartToPolar(target_sobel_x, target_sobel_y, target_sobel_mag, new Mat());
            CvInvoke.CartToPolar(texture_sobel_x, texture_sobel_y, texture_sobel_mag, new Mat());


            List<Matrix<float>> transformation_matrixs = new List<Matrix<float>>();
            List<Matrix<float>> transformation_matrixs_invert = new List<Matrix<float>>();
            List<RectangleF> rotatedRects = new List<RectangleF>();

            for (int i = 1; i < rotations; i++)
            {
                double angle = i * (360.0 / (float)rotations);
                RectangleF rotatedRect = new RotatedRect(new PointF(), texture.Size, (float)angle).MinAreaRect();
                PointF center = new PointF(0.5f * texture.Width, 0.5f * texture.Height);
                Matrix<float> transformation_matrix = new Matrix<float>(2, 3);
                Matrix<float> transformation_matrix_invert = new Matrix<float>(2, 3);
                CvInvoke.GetRotationMatrix2D(center, angle, 1.0, transformation_matrix);

                transformation_matrix.Data[0, 2] += (rotatedRect.Width - texture.Width) / 2;
                transformation_matrix.Data[1, 2] += (rotatedRect.Height - texture.Height) / 2;
                CvInvoke.InvertAffineTransform(transformation_matrix, transformation_matrix_invert);
                transformation_matrixs.Add(transformation_matrix);
                transformation_matrixs_invert.Add(transformation_matrix_invert);
                rotatedRects.Add(rotatedRect);
            }

            List<Image<Bgr, byte>> texture_rotations = new List<Image<Bgr, byte>>(rotations) { };
            List<Image<Gray, byte>> texture_gray_rotations = new List<Image<Gray, byte>>(rotations) { };
            List<Image<Gray, float>> texture_sobel_rotations = new List<Image<Gray, float>>(rotations) { };
            List<Image<Gray, byte>> texture_mask_rotations = new List<Image<Gray, byte>>(rotations) { };

            texture_rotations.Add(texture);
            texture_gray_rotations.Add(texture_gray);
            texture_sobel_rotations.Add(texture_sobel_mag);
            texture_mask_rotations.Add(new Image<Gray, byte>(texture.Width, texture.Height, new Gray(255)));
            for (int i = 1; i < rotations; i++)
            {
                texture_mask_rotations.Add(new Image<Gray, byte>(rotatedRects[i - 1].Size.ToSize()));
                texture_rotations.Add(new Image<Bgr, byte>(rotatedRects[i - 1].Size.ToSize()));
                texture_gray_rotations.Add(new Image<Gray, byte>(rotatedRects[i - 1].Size.ToSize()));
                texture_sobel_rotations.Add(new Image<Gray, float>(rotatedRects[i - 1].Size.ToSize()));
            }
            for (int i = 1; i < rotations; i++)
            {
                CvInvoke.WarpAffine(texture, texture_rotations[i], transformation_matrixs[i - 1], rotatedRects[i - 1].Size.ToSize(), Inter.Nearest, Warp.Default, BorderType.Constant, new MCvScalar(0));
                CvInvoke.WarpAffine(texture_gray, texture_gray_rotations[i], transformation_matrixs[i - 1], rotatedRects[i - 1].Size.ToSize(), Inter.Nearest, Warp.Default, BorderType.Constant, new MCvScalar(0));
                CvInvoke.WarpAffine(texture_sobel_mag, texture_sobel_rotations[i], transformation_matrixs[i - 1], rotatedRects[i - 1].Size.ToSize(), Inter.Nearest, Warp.Default, BorderType.Constant, new MCvScalar(0));
                CvInvoke.WarpAffine(texture_mask_rotations[0], texture_mask_rotations[i], transformation_matrixs[i - 1], rotatedRects[i - 1].Size.ToSize(), Inter.Nearest, Warp.Default, BorderType.Constant, new MCvScalar(0));
            }

            // Directory.SetCurrentDirectory(path);
            String current_path = path + @"\matched_patches";
            if (Directory.Exists(current_path))
            {
                DirectoryInfo di = new DirectoryInfo(current_path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            else
            {
                DirectoryInfo di = Directory.CreateDirectory(current_path);
            }

            List<Rectangle> target_patches = new List<Rectangle>();
            for (int y = 0; y < target_hist_matched_weighted.Height / size; y++)
            {
                for (int x = 0; x < target_hist_matched_weighted.Width / size; x++)
                {
                    target_patches.Add(new Rectangle(x * size, y * size, size, size));
                }
            }
            Console.WriteLine(new Point(target.Width / 2, target.Height / 2));
            target_patches.Sort((Rectangle i, Rectangle j) =>
            {
                Point center_i = new Point(i.X + i.Width / 2, i.Y + i.Height / 2);
                Point center_j = new Point(j.X + j.Width / 2, j.Y + j.Height / 2);
                Point origin = new Point(target.Width / 2, target.Height / 2);

                double dist_i = Math.Pow(center_i.X - origin.X, 2) + Math.Pow(center_i.Y - origin.Y, 2);
                double dist_j = Math.Pow(center_j.X - origin.X, 2) + Math.Pow(center_j.Y - origin.Y, 2);
                Console.WriteLine($"i: {center_i}, j:{center_j}");
                if (dist_i > dist_j)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }

            });


            foreach (var template_rect in target_patches)
            {               
                int x = template_rect.X / size;
                int y = template_rect.Y / size;
                Image<Bgr, byte> template;
                Image<Gray, byte> template_gray;
                Image<Gray, float> template_sobel;

                template = target.Clone();
                template_gray = target_hist_matched_weighted.Clone();
                template_sobel = target_sobel_mag.Clone();
                template.ROI = template_rect;
                template_gray.ROI = template_rect;
                template_sobel.ROI = template_rect;
                template = template.Clone();
                template_gray = template_gray.Copy();
                template_sobel = template_sobel.Copy();

                int minMatchIndex = -1;
                double minMatchValue = double.MaxValue;
                Point minMatchLoc = new Point();
                Object _lock = new Object();
                Parallel.For(0, rotations, i =>
                {
                    Image<Gray, float> match_gray = new Image<Gray, float>(texture_gray.Size);
                    Image<Gray, float> match_sobel = new Image<Gray, float>(texture_sobel_mag.Size);
                    Image<Gray, float> match_sum = new Image<Gray, float>(texture.Size);
                    double minVal = 0, maxVal = 0;
                    Point minLoc = new Point(), maxLoc = new Point();

                    CvInvoke.MatchTemplate(texture_gray_rotations[i], template_gray, match_gray, TemplateMatchingType.Sqdiff);
                    CvInvoke.MatchTemplate(texture_sobel_rotations[i], template_sobel, match_sobel, TemplateMatchingType.Sqdiff);
                    match_sum = match_gray + match_sobel;
                    //CvInvoke.MinMaxLoc(match_sum, ref minVal, ref maxVal, ref minLoc, ref maxLoc);
                    //CudaInvoke.MinMaxLoc(match_sum, ref minVal, ref maxVal, ref minLoc, ref maxLoc, GetMinMaxLocMask(texture_mask_rotations[i], size));
                    CvInvoke.MinMaxLoc(match_sum, ref minVal, ref maxVal, ref minLoc, ref maxLoc, GetMinMaxLocMask(texture_mask_rotations[i], size));
                    lock (_lock)
                    {
                        if (minVal < minMatchValue && minVal > 0)
                        {
                            minMatchValue = minVal;
                            minMatchIndex = i;
                            minMatchLoc = minLoc;
                        }
                    }
                    match_gray.Dispose();
                    match_sobel.Dispose();
                    match_sum.Dispose();
                });
                Console.WriteLine($"minMatchValue = {minMatchValue}\r\nminMatchIndex = {minMatchIndex}\r\nminMatchLoc = {minMatchLoc}");
                if (minMatchIndex < 0)
                {
                    MessageBox.Show("Out of textures!!!!");
                    return;
                }
                texture_mask_rotations[minMatchIndex].Draw(new Rectangle(minMatchLoc.X, minMatchLoc.Y, size - 1, size - 1), new Gray(0), -1);
                if (minMatchIndex > 0)
                {
                    Image<Gray, byte> mask = new Image<Gray, byte>(texture_mask_rotations[0].Size);
                    CvInvoke.WarpAffine(texture_mask_rotations[minMatchIndex], mask, transformation_matrixs_invert[minMatchIndex - 1], mask.Size, Inter.Nearest, Warp.Default, BorderType.Constant, new MCvScalar(0));
                    texture_mask_rotations[0] = texture_mask_rotations[0] - (255 - mask);
                }
                for (int i = 1; i < rotations; i++)
                {
                    Image<Gray, byte> mask_rot = new Image<Gray, byte>(texture_mask_rotations[i].Size);
                    CvInvoke.WarpAffine(texture_mask_rotations[0], mask_rot, transformation_matrixs[i - 1], mask_rot.Size, Inter.Nearest, Warp.Default, BorderType.Constant, new MCvScalar(0));
                    mask_rot.CopyTo(texture_mask_rotations[i]);
                }

                texture_rotations[minMatchIndex].ROI = new Rectangle(minMatchLoc, new Size(size, size));
                CvInvoke.Imwrite($@"{current_path}\{x}_{y}.bmp", texture_rotations[minMatchIndex].Copy());
                imageBox_match.Image = texture_rotations[minMatchIndex].Copy();
                imageBox_template.Image = template.Copy();

                Image<Bgr, byte> target_temp = target.Clone();
                target_temp.Draw(template_rect, new Bgr(0, 0, 255), 5);
                imageBox_Target.Image = target_temp;
                target_temp.Dispose();

                Image_Result.ROI = new Rectangle(x * size, y * size, size, size);
                texture_rotations[minMatchIndex].CopyTo(Image_Result);
                Image_Result.ROI = Rectangle.Empty;
                texture_rotations[minMatchIndex].ROI = Rectangle.Empty;

                imageBox_Result.Image = Image_Result;


                if (minMatchIndex > 0)
                {
                    PointF[] maskPoints = { minMatchLoc, new PointF(minMatchLoc.X + size - 1, minMatchLoc.Y), new PointF(minMatchLoc.X, minMatchLoc.Y + size - 1), new PointF(minMatchLoc.X + size - 1, minMatchLoc.Y + size - 1) };
                    PointF[] maskPoints_rot = new PointF[4];
                    for (int i = 0; i < 4; i++)
                    {
                        maskPoints_rot[i].X = maskPoints[i].X * transformation_matrixs_invert[minMatchIndex - 1].Data[0, 0] + maskPoints[i].Y * transformation_matrixs_invert[minMatchIndex - 1].Data[0, 1] + 1.0f * transformation_matrixs_invert[minMatchIndex - 1].Data[0, 2];
                        maskPoints_rot[i].Y = maskPoints[i].X * transformation_matrixs_invert[minMatchIndex - 1].Data[1, 0] + maskPoints[i].Y * transformation_matrixs_invert[minMatchIndex - 1].Data[1, 1] + 1.0f * transformation_matrixs_invert[minMatchIndex - 1].Data[1, 2];
                    }
                    Point[] maskPoints_rot_round = { Point.Round(maskPoints_rot[0]), Point.Round(maskPoints_rot[1]), Point.Round(maskPoints_rot[3]), Point.Round(maskPoints_rot[2]) };
                    texture.FillConvexPoly(maskPoints_rot_round, new Bgr(0, 0, 0));
                }
                else
                {
                    //texture.DrawPolyline(new Point[] { minMatchLoc, new Point(minMatchLoc.X + size, minMatchLoc.Y), new Point(minMatchLoc.X + size, minMatchLoc.Y + size), new Point(minMatchLoc.X, minMatchLoc.Y + size) }, true, new Bgr(0, 0, 0), 0);
                    texture.Draw(new Rectangle(minMatchLoc, new Size(size - 1, size - 1)), new Bgr(0, 0, 0), -1);
                }
                imageBox_Texture.Image = texture;

                template.Dispose();
                template_gray.Dispose();
                template_sobel.Dispose();
                GC.Collect();

                this.Invoke(new MethodInvoker(() =>
                {
                    progressBar_match.Value++;
                }));
                //MessageBox.Show("");
            }           

        }
        private void GeneratePatches()
        {
            String current_path = path + @"\source_patches";
            Image<Bgr, Byte> target = Image_Target.Clone();

            if (Directory.Exists(current_path))
            {
                DirectoryInfo di = new DirectoryInfo(current_path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            else
            {
                DirectoryInfo di = Directory.CreateDirectory(current_path);
            }

            for (int y = 0; y < target.Height / size; y++)
            {
                for (int x = 0; x < target.Width / size; x++)
                {
                    Rectangle ROI = new Rectangle(x * size, y * size, size, size);
                    target.ROI = ROI;
                    CvInvoke.Imwrite($@"{current_path}\{x}_{y}.bmp", target.Copy());
                    target.ROI = Rectangle.Empty;
                }
            }
            target.Dispose();
        }
        private void MergePatches()
        {
            DirectoryInfo di = new DirectoryInfo(path + @"\matched_patches");
            //DirectoryInfo di = new DirectoryInfo(@"C:\patches");
            List<String> imgNames = new List<string>();
            List<Image<Bgr, byte>> patches = new List<Image<Bgr, byte>>();

            foreach (FileInfo file in di.GetFiles())
            {
                Console.WriteLine(file.FullName);
                imgNames.Add(file.FullName);
                //CvInvoke.Imshow("", new Image<Bgr, byte>(file.FullName));
            }

            imgNames.Sort((string x, string y) =>
            {
                string[] split_x = x.Split('\\');
                string[] split_y = y.Split('\\');
                string filename_x = split_x.Last();
                string filename_y = split_y.Last();

                int row_x = Convert.ToInt32(filename_x.Split('_', '.')[0]);
                int col_x = Convert.ToInt32(filename_x.Split('_', '.')[1]);
                int row_y = Convert.ToInt32(filename_y.Split('_', '.')[0]);
                int col_y = Convert.ToInt32(filename_y.Split('_', '.')[1]);

                if (row_x > row_y)
                {
                    return 1;
                }
                else if (row_x == row_y)
                {
                    if (col_x > col_y)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            });

            for (int i = 0; i < imgNames.Count; i++)
            {
                patches.Add(new Image<Bgr, byte>(imgNames[i]));
            }

            int max_row = Convert.ToInt32(imgNames.Last().Split('\\').Last().Split('_', '.')[0]) + 1;
            int max_col = Convert.ToInt32(imgNames.Last().Split('\\').Last().Split('_', '.')[1]) + 1;

            Console.WriteLine(max_row);
            Console.WriteLine(max_col);
            Mat img_mat = new Mat();
            for (int y = 0; y < max_row; y++)
            {
                Console.WriteLine($"y: {y}");
                Mat H_mat = patches[y * max_col].Mat.Clone();
                for (int x = 1; x < max_col; x++)
                {
                    Console.WriteLine($"x: {x}");
                    CvInvoke.VConcat(H_mat, patches[y * max_col + x], H_mat);

                }
                if (y == 0)
                {
                    img_mat = H_mat.Clone();
                }
                else
                {
                    CvInvoke.HConcat(img_mat, H_mat, img_mat);
                }

            }

            //CvInvoke.Imshow($"final", img_mat.ToImage<Bgr, byte>());

            imageBox_Result.Image = img_mat.ToImage<Bgr, byte>();
        }


        private void button_LoadTarget_Click(object sender, EventArgs e)
        {
            DialogResult result;

            openFileDialog.Title = "Select Target Image...";
            result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (openFileDialog.FileName == null)
                {
                    return;
                }
                Image_Target = new Image<Bgr, byte>(openFileDialog.FileName);
                imageBox_Target.Image = Image_Target;
            }

            if (Image_Target == null || Image_Texture == null)
            {
                return;
            }
            else
            {
                PreviewSettings();
                button_hist_Click(null, null);
            }
        }

        private void button_LoadTexture_Click(object sender, EventArgs e)
        {
            DialogResult result;

            openFileDialog.Title = "Select Texture Image...";
            result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (openFileDialog.FileName == null)
                {
                    return;
                }
                Image_Texture = new Image<Bgr, byte>(openFileDialog.FileName);
                imageBox_Texture.Image = Image_Texture;
            }
            if (Image_Target == null || Image_Texture == null)
            {
                return;
            }
            else
            {
                PreviewSettings();
                button_hist_Click(null, null);
            }
        }

        Image<Gray, byte> GetMinMaxLocMask(Image<Gray, byte> src, int kernel_size)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(src.Width - kernel_size + 1, src.Height - kernel_size + 1);

            Mat kernel1 = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Cross, new Size(kernel_size, kernel_size), new Point(0, 0));
            Mat kernel2 = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Cross, new Size(kernel_size, kernel_size), new Point(kernel_size - 1, kernel_size - 1));
            Image<Gray, byte> mask = src.Clone();
            Image<Gray, byte> mask_erode1 = mask.Clone();
            Image<Gray, byte> mask_erode2 = mask.Clone();
            //CvInvoke.Erode(mask, mask_erode, kernel, new Point(0, 0), 1, BorderType.Constant, new MCvScalar(0));
            //Rectangle kernel = new Rectangle(0, 0, 50, 50);
            CvInvoke.Erode(mask, mask_erode1, kernel1, new Point(0, 0), 1, BorderType.Constant, new MCvScalar(0));
            CvInvoke.Erode(mask, mask_erode2, kernel2, new Point(0, 0), 1, BorderType.Constant, new MCvScalar(0));
            mask_erode2 = mask_erode2 - (255 - mask_erode1);
            mask_erode2.ROI = new Rectangle(new Point(0, 0), result.Size);
            result = mask_erode2.Copy();

            return result;
        }

        Image<Gray, byte> GetMinMaxLocMask2(Image<Gray, byte> src, int kernel_size)
        {
            Image<Gray, byte> result_corp = new Image<Gray, byte>(src.Width - kernel_size + 1, src.Height - kernel_size + 1);
            Image<Gray, byte> mask = src;
            Image<Gray, byte> result = src;
            for (int y = 0; y < kernel_size; y++)
            {
                for (int x = 0; x < kernel_size; x++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    Image<Gray, byte> mask_shift = ShiftImage(mask, -x, -y);
                    result = result - (255 - mask_shift);
                    mask_shift.Dispose();
                }
            }
            result.ROI = new Rectangle(new Point(0, 0), result_corp.Size);
            result_corp = result.Copy();

            return result_corp;
        }

        Image<Gray, byte> GetMinMaxLocMask3(Image<Gray, byte> src, int kernel_size)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(src.Width - kernel_size + 1, src.Height - kernel_size + 1);
            Image<Gray, byte> mask = src.Clone();
            Image<Gray, byte> mask_shift = src.Clone();

            Parallel.For(0, mask.Height, y =>
            {
                for (int x = 0; x < mask.Width; x++)
                {
                    if (mask.Data[y, x, 0] == 0)
                    {
                        mask_shift.Draw(new Rectangle(x - kernel_size + 1, y - kernel_size + 1, kernel_size, kernel_size), new Gray(0), -1);
                    }
                }
            });
            mask = mask - (255 - mask_shift);
            mask.ROI = new Rectangle(new Point(0, 0), result.Size);
            result = mask.Copy();

            return result;
        }
        Image<Gray, byte> ShiftImage(Image<Gray, byte> src, int offset_x, int offset_y)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(src.Size);
            Matrix<float> shiiftingMatrix = new Matrix<float>(2, 3);
            shiiftingMatrix[0, 0] = 1;
            shiiftingMatrix[0, 1] = 0;
            shiiftingMatrix[0, 2] = offset_x;
            shiiftingMatrix[1, 0] = 0;
            shiiftingMatrix[1, 1] = 1;
            shiiftingMatrix[1, 2] = offset_y;
            CvInvoke.WarpAffine(src, result, shiiftingMatrix, src.Size, Inter.Linear, Warp.Default, BorderType.Constant, new MCvScalar(255));
            return result;
        }


        private void trackBar_size_Scroll(object sender, EventArgs e)
        {
            label_size.Text = $"Patch Size({trackBar_size.Value}*{trackBar_size.Value}):";
            PreviewSettings();
        }

        private void trackBar_Rotations_Scroll(object sender, EventArgs e)
        {
            label_Rotations.Text = $"Number of Rotations ({trackBar_Rotations.Value}):";
            PreviewSettings();
        }

        private void trackBar_scale_Scroll(object sender, EventArgs e)
        {
            label_scale.Text = $"Taget Image Scale ({((float)trackBar_scale.Value) / 10.0f,1:N1}x):";
            PreviewSettings();
        }

        private void trackBar_hist_Scroll(object sender, EventArgs e)
        {
            label_hist.Text = $"Hist. match weight ({((float)trackBar_hist.Value) / 10.0f,1:N1}x):";
        }
        private void PreviewSettings()
        {
            if (Image_Target == null || Image_Texture == null)
            {
                return;
            }
            Image<Bgr, byte> target = Image_Target.Clone();
            Image<Bgr, byte> texture = Image_Texture.Clone();

            int size_temp = trackBar_size.Value;
            double scale_temp = (double)trackBar_scale.Value / 10.0;

            target = target.Resize(scale_temp, Inter.Linear);
            Rectangle patch_rect = new Rectangle(0, 0, size_temp, size_temp);

            target.Draw(patch_rect, new Bgr(0, 0, 255), 5);
            texture.Draw(patch_rect, new Bgr(0, 0, 255), 5);

            imageBox_Target.Image = target;
            imageBox_Texture.Image = texture;
        }

        private void button_hist_Click(object sender, EventArgs e)
        {
            if (Image_Target == null || Image_Texture == null)
            {
                return;
            }
            double temp_weight = (double)trackBar_hist.Value / 10.0;
            Image<Gray, Byte> target_gray = Image_Target.Clone().Convert<Gray, Byte>();
            Image<Gray, Byte> texture_gray = Image_Texture.Clone().Convert<Gray, Byte>();
            Image<Gray, Byte> target_hist_matched = new Image<Gray, Byte>(Image_Target.Size);
            Image<Gray, Byte> target_hist_matched_weighted = new Image<Gray, Byte>(Image_Texture.Size);

            Matrix<byte> histLUT = new Matrix<byte>(1, 256);
            Mat hist_target = new Mat();
            Mat hist_texture = new Mat();

            VectorOfMat vm_target = new VectorOfMat();
            VectorOfMat vm_texture = new VectorOfMat();
            vm_target.Push(target_gray);
            vm_texture.Push(texture_gray);

            CvInvoke.CalcHist(vm_target, new int[] { 0 }, null, hist_target, new int[] { 256 }, new float[] { 0, 255 }, false);
            CvInvoke.CalcHist(vm_texture, new int[] { 0 }, null, hist_texture, new int[] { 256 }, new float[] { 0, 255 }, false);

            float[] CDF_hist_target = new float[256];
            float[] CDF_hist_texture = new float[256];
            Marshal.Copy(hist_target.DataPointer, CDF_hist_target, 0, 256);
            Marshal.Copy(hist_texture.DataPointer, CDF_hist_texture, 0, 256);

            for (int i = 1; i < 256; i++)
            {
                CDF_hist_target[i] += CDF_hist_target[i - 1];
                CDF_hist_texture[i] += CDF_hist_texture[i - 1];
            }

            for (int i = 0; i < 256; i++)
            {
                histLUT.Data[0, i] = 0;
                for (int j = 0; j < 256; j++)
                {
                    if (CDF_hist_texture[j] >= CDF_hist_target[i])
                    {
                        histLUT.Data[0, i] = (byte)j;
                        break;
                    }
                }
            }
            CvInvoke.LUT(target_gray, histLUT, target_hist_matched);
            target_hist_matched_weighted = target_hist_matched * temp_weight + target_gray * (1.0 - temp_weight);
            imageBox_hist.Image = target_hist_matched_weighted;
        }
    }
}

