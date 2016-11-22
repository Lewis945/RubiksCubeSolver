using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using NUnit.Framework;
using RubiksCube.OpenCV.TestCase.AugmentedReality;
using RubiksCube.OpenCV.TestCase.PtamLikeApproach;

using Accord.Math;
using Accord.Math.Decompositions;
using Accord.Statistics;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;

namespace RubiksCube.OpenCV.TestCase.Tests
{
    [TestFixture]
    public class SimpleAdHocTrackerTests
    {
        private readonly CameraCalibrationInfo _calibration;
        private readonly SimpleAdHocTracker _tracker;

        private static readonly int StartinPoint = 40;
        private static readonly int BootstrappedPoint = 95;

        //private static readonly string TestCaseProjectPath = @"C:\Users\zakharov\Documents\Repos\Mine\Rc\src\RubiksCube.OpenCV.TestCase";
        private static readonly string TestCaseProjectPath = @"D:\Projects\RubiksCube\src\RubiksCube.OpenCV.TestCase";
        //private static readonly string TestCaseTestProjectPath = "C:/Users/zakharov/Documents/Repos/Mine/Rc/tests/RubiksCube.OpenCV.TestCase.Tests";
        private static readonly string TestCaseTestProjectPath = @"D:\Projects\RubiksCube\tests\RubiksCube.OpenCV.TestCase.Tests";

        public SimpleAdHocTrackerTests()
        {
            _calibration = new CameraCalibrationInfo(560.764656335266f, 562.763179958161f, 295.849138757436f, 255.022208986073f);
            _tracker = new SimpleAdHocTracker(_calibration);

            Trace.Listeners.Add(new TextWriterTraceListener($"{TestCaseTestProjectPath}/Logs/log {DateTime.Now:H-mm-ss dd-MM-yyyy}.txt", "tracelog"));
        }

        [Test]
        public void Bootstrap_Track_Test()
        {
            var capture = new Capture($@"{TestCaseProjectPath}\Videos\cube2.avi");

            int i = 0;
            do
            {
                using (var currentFrame = capture.QueryFrame())
                {
                    if (currentFrame == null)
                        break;

                    if (i == StartinPoint)
                    {
                        _tracker.Process(currentFrame, true);
                    }
                    else if (i > StartinPoint)
                    {
                        _tracker.Process(currentFrame, false);

                        if (i == BootstrappedPoint)
                        {
                            CvInvoke.PutText(currentFrame, i.ToString(), new Point(20, 20), FontFace.HersheyPlain, 2,
                                new MCvScalar(100, 10, 100));

                            currentFrame.Save("output.jpg");

                            break;
                        }
                    }

                    i++;
                }
            }
            while (true);
        }

        [Test]
        public void Parse_CPlusPlus_File()
        {
            var parsed = new Dictionary<string, List<string>>();

            using (
                var reader = File.OpenText(
                        $@"{TestCaseTestProjectPath}\out.txt"))
            {
                string currentKey = "init";
                parsed.Add(currentKey, new List<string>());

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("I ="))
                    {
                        currentKey = line;
                        if (!parsed.ContainsKey(line))
                            parsed.Add(line, new List<string>());
                    }

                    parsed[currentKey].Add(line);
                }
            }

            foreach (var item in parsed)
            {
                File.WriteAllLines($@"{TestCaseTestProjectPath}\cpluspluslogs\{item.Key}.txt", item.Value);
            }
        }

        [Test]
        public void Create_CSharp_Files_From_CPlusPlus_Files()
        {
            var parsed = new Dictionary<string, List<string>>();
            string currentKey = "init";
            parsed.Add(currentKey, new List<string>());

            var dir = new DirectoryInfo(
                    $@"{TestCaseTestProjectPath}\cpluspluslogs\");

            foreach (var file in dir.GetFiles())
            {
                using (var reader = File.OpenText(file.FullName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("Bootstrap points before optical flow"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - Bootstrap points before optical flow";
                            parsed.Add(currentKey, new List<string>());
                        }
                        else if (line.Contains("Tracked points before optical flow"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - Tracked points before optical flow";
                            parsed.Add(currentKey, new List<string>());
                        }
                        else if (line.Contains("Bootstrap points after optical flow"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - Bootstrap points after optical flow";
                            parsed.Add(currentKey, new List<string>());
                        }
                        else if (line.Contains("Tracked points after optical flow"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - Tracked points after optical flow";
                            parsed.Add(currentKey, new List<string>());
                        }
                        else if (line.Contains("Homography mask"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - Homography mask";
                            parsed.Add(currentKey, new List<string>());
                        }
                        else if (line.Contains("Homography"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - Homography";
                            parsed.Add(currentKey, new List<string>());
                        }
                        else if (line.Contains("Bootstrap points after homography"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - Bootstrap points after homography";
                            parsed.Add(currentKey, new List<string>());
                        }
                        else if (line.Contains("Tracked points after homography"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - Tracked points after homography";
                            parsed.Add(currentKey, new List<string>());
                        }
                        else if (line.Contains("3d points"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - 3d points";
                            parsed.Add(currentKey, new List<string>());
                        }
                        else if (line.Contains("2nd row eigenvectors"))
                        {
                        }
                        else if (line.Contains("eigenvectors"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - eigenvectors";
                            parsed.Add(currentKey, new List<string>());
                        }
                        else if (line.Contains("normal of plane"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - normal of plane";
                            parsed.Add(currentKey, new List<string>());
                        }
                        else if (line.Contains("p_to_plane_thresh"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - p_to_plane_thresh";
                            parsed.Add(currentKey, new List<string>());
                            parsed[currentKey].Add(line.Remove(0, line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("num inliers"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - num inliers";
                            parsed.Add(currentKey, new List<string>());
                            parsed[currentKey].Add(line.Remove(0, line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("status arr"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - status arr";
                            parsed.Add(currentKey, new List<string>());
                        }
                        else if (line.Contains("trackedFeatures3DM"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - trackedFeatures3DM";
                            parsed.Add(currentKey, new List<string>());
                        }
                        else if (line.Contains("projected trackedFeatures3mat"))
                        {
                            currentKey = $"{file.Name.Replace(".", "")} - projected trackedFeatures3mat";
                            parsed.Add(currentKey, new List<string>());
                        }
                        else if (line.Contains("features survived optical flow") ||
                                 line.Contains("features survived homography"))
                        {
                            continue;
                        }
                        else
                        {
                            parsed[currentKey].Add(line);
                        }

                        if (line.Contains("[") && line.IndexOf("[", StringComparison.Ordinal) > 2)
                        {
                            line = line.Remove(0, line.IndexOf("[", StringComparison.Ordinal));
                            parsed[currentKey].Add(line);
                        }

                        if (line.Contains("]") && line.IndexOf("]", StringComparison.Ordinal) == line.Length - 1)
                        {
                            currentKey = "init";
                        }
                    }
                }
                currentKey = "init";
            }

            foreach (var item in parsed)
            {
                File.WriteAllLines($@"{TestCaseTestProjectPath}\csharplogs\{item.Key}.txt", item.Value);
            }
        }

        [Test]
        public void Bootstrap_Track_Logic_Test()
        {
            var capture = new Capture($@"{TestCaseProjectPath}\Videos\cube2.avi");
            //var capture = new Capture(@"C:\Users\zakharov\Documents\Repos\Mine\Rc\src\RubiksCube.OpenCV.TestCase\Videos\cube2.avi");
            for (int i = 0; i < 40; i++)
            {
                capture.QueryFrame();
            }

            var prevGray = capture.QueryFrame();
            CvInvoke.CvtColor(prevGray, prevGray, ColorConversion.Bgr2Gray);

            var currentGray = capture.QueryFrame();
            CvInvoke.CvtColor(currentGray, currentGray, ColorConversion.Bgr2Gray);

            var bootstrapKp = new VectorOfKeyPoint();
            new ORBDetector().DetectRaw(prevGray, bootstrapKp);

            var trackedFeatures = new VectorOfKeyPoint(bootstrapKp.ToArray());
            VectorOfPoint3D32F trackedFeatures3D;

            //-------------------------------------------------------------------------

            var pointComparer = Comparer<PointF>.Create((p1, p2) => Math.Abs(p1.X - p2.X) < 0.0001f && Math.Abs(p1.Y - p2.Y) < 0.0001f ? 0 : 1);
            var point3DComparer = Comparer<MCvPoint3D32f>.Create((p1, p2) => Math.Abs(p1.X - p2.X) < 0.0001f && Math.Abs(p1.Y - p2.Y) < 0.0001f && Math.Abs(p1.Z - p2.Z) < 0.0001f ? 0 : 1);
            var matrixComparer = Comparer<double>.Create((x, y) => Math.Abs(x - y) < 0.01f ? 0 : 1);

            VectorOfPointF bootstrapPointsBeforeOpticalFlowCplusPlus;
            VectorOfPointF trackedPointsBeforeOpticalFlowCplusPlus;
            VectorOfPointF bootstrapPointsAfterOpticalFlowCplusPlus;
            VectorOfPointF trackedPointsAfterOpticalFlowCplusPlus;
            VectorOfPointF bootstrapPointsAfterHomographyCplusPlus;
            VectorOfPointF trackedPointsAfterHomographyCplusPlus;

            Matrix<double> homographyCplusPlus;
            VectorOfByte homographyMaskCplusPlus;

            VectorOfPoint3D32F points3dCplusPlus;
            Matrix<double> eigenvectorsCplusPlus;
            double[] normalOfPlaneCplusPlus;

            double pToPlaneTrashCplusPlus;
            int numInliersCplusPlus;
            VectorOfByte statusArrCplusPlus;
            Matrix<double> trackedFeatures3DMCplusPlus;
            Matrix<double> projectedTrackedFeaturesCplusPlus;

            for (int i = 41; i <= 95; i++)
            {
                bootstrapPointsBeforeOpticalFlowCplusPlus = GetPoints($"I = {i}txt - Bootstrap points before optical flow.txt");
                trackedPointsBeforeOpticalFlowCplusPlus = GetPoints($"I = {i}txt - Tracked points before optical flow.txt");
                bootstrapPointsAfterOpticalFlowCplusPlus = GetPoints($"I = {i}txt - Bootstrap points after optical flow.txt");
                trackedPointsAfterOpticalFlowCplusPlus = GetPoints($"I = {i}txt - Tracked points after optical flow.txt");
                bootstrapPointsAfterHomographyCplusPlus = GetPoints($"I = {i}txt - Bootstrap points after homography.txt");
                trackedPointsAfterHomographyCplusPlus = GetPoints($"I = {i}txt - Tracked points after homography.txt");

                homographyCplusPlus = Getmatrix3X3($"I = {i}txt - Homography.txt");
                homographyMaskCplusPlus = GetByteVector($"I = {i}txt - Homography mask.txt");

                var corners = new VectorOfPointF();
                var status = new VectorOfByte();
                var errors = new VectorOfFloat();

                CollectionAssert.AreEqual(Utils.GetPointsVector(bootstrapKp).ToArray(), bootstrapPointsBeforeOpticalFlowCplusPlus.ToArray(), pointComparer);
                CollectionAssert.AreEqual(Utils.GetPointsVector(trackedFeatures).ToArray(), trackedPointsBeforeOpticalFlowCplusPlus.ToArray(), pointComparer);

                CvInvoke.CalcOpticalFlowPyrLK(prevGray, currentGray, Utils.GetPointsVector(trackedFeatures), corners,
                    status, errors, new Size(11, 11), 3, new MCvTermCriteria(20, 0.03));
                currentGray.CopyTo(prevGray);

                if (CvInvoke.CountNonZero(status) < status.Size * 0.8)
                {
                    throw new Exception("Tracking failed.");
                }

                //trackedFeatures = Utils.GetKeyPointsVector(corners);
                trackedFeatures = Utils.GetKeyPointsVector(trackedPointsAfterOpticalFlowCplusPlus);

                Utils.KeepVectorsByStatus(ref trackedFeatures, ref bootstrapKp, status);

                CollectionAssert.AreEqual(Utils.GetPointsVector(bootstrapKp).ToArray(), bootstrapPointsAfterOpticalFlowCplusPlus.ToArray(), pointComparer);
                CollectionAssert.AreEqual(Utils.GetPointsVector(trackedFeatures).ToArray(), trackedPointsAfterOpticalFlowCplusPlus.ToArray(), pointComparer);

                if (trackedFeatures.Size != bootstrapKp.Size)
                {
                    const string error = "Tracked features vector size is not equal to bootstrapped one.";
                    throw new Exception(error);
                }

                //verify features with a homography
                var inlierMask = new VectorOfByte();
                var homography = new Mat();
                if (trackedFeatures.Size > 4)
                    CvInvoke.FindHomography(Utils.GetPointsVector(trackedFeatures), Utils.GetPointsVector(bootstrapKp), homography, HomographyMethod.Ransac, 0.99,
                        inlierMask);

                var homographyMatrix = new Matrix<double>(homography.Rows, homography.Cols, homography.DataPointer);
                CollectionAssert.AreEqual(homographyMatrix.Data, homographyCplusPlus.Data, matrixComparer);

                int inliersNum = CvInvoke.CountNonZero(inlierMask);
                CollectionAssert.AreEqual(inlierMask.ToArray(), homographyMaskCplusPlus.ToArray());

                if (inliersNum != trackedFeatures.Size && inliersNum >= 4 && !homography.IsEmpty)
                {
                    Utils.KeepVectorsByStatus(ref trackedFeatures, ref bootstrapKp, inlierMask);
                }
                else if (inliersNum < 10)
                {
                    throw new Exception("Not enough features survived homography.");
                }

                CollectionAssert.AreEqual(Utils.GetPointsVector(bootstrapKp).ToArray(), bootstrapPointsAfterHomographyCplusPlus.ToArray(), pointComparer);
                CollectionAssert.AreEqual(Utils.GetPointsVector(trackedFeatures).ToArray(), trackedPointsAfterHomographyCplusPlus.ToArray(), pointComparer);

                var bootstrapKpOrig = new VectorOfKeyPoint(bootstrapKp.ToArray());
                var trackedFeaturesOrig = new VectorOfKeyPoint(trackedFeatures.ToArray());

                //TODO: Compare all these to c++ version
                //Attempt at 3D reconstruction (triangulation) if conditions are right
                var rigidT = CvInvoke.EstimateRigidTransform(Utils.GetPointsVector(trackedFeatures).ToArray(), Utils.GetPointsVector(bootstrapKp).ToArray(), false);
                var matrix = new Matrix<double>(rigidT.Rows, rigidT.Cols, rigidT.DataPointer);

                if (CvInvoke.Norm(matrix.GetCol(2)) > 100)
                {
                    points3dCplusPlus = GetPoints3d($"I = {i}txt - 3d points.txt");
                    eigenvectorsCplusPlus = Getmatrix3X3($"I = {i}txt - eigenvectors.txt");
                    normalOfPlaneCplusPlus = GetDoubleArray($"I = {i}txt - normal of plane.txt");

                    //camera motion is sufficient
                    var result = OpenCvUtilities.CameraPoseAndTriangulationFromFundamental(_calibration, trackedFeatures, bootstrapKp);

                    trackedFeatures = result.FilteredTrackedFeaturesKp;
                    bootstrapKp = result.FilteredBootstrapKp;

                    if (result.Result)
                    {
                        pToPlaneTrashCplusPlus = GetDouble($"I = {i}txt - p_to_plane_thresh.txt");
                        numInliersCplusPlus = GetInt($"I = {i}txt - num inliers.txt");
                        statusArrCplusPlus = GetByteVector($"I = {i}txt - status arr.txt");

                        trackedFeatures3D = result.TrackedFeatures3D;

                        CollectionAssert.AreEqual(trackedFeatures3D.ToArray(), points3dCplusPlus.ToArray(), point3DComparer);

                        //var trackedFeatures3Dm = Utils.Get3dPointsMat(trackedFeatures3D);
                        var tf3D = new double[trackedFeatures3D.Size, 3];
                        var trackedFeatures3Dm = new Matrix<double>(trackedFeatures3D.Size, 3);
                        for (int k = 0; k < trackedFeatures3D.Size; k++)
                        {
                            trackedFeatures3Dm[k, 0] = trackedFeatures3D[k].X;
                            trackedFeatures3Dm[k, 1] = trackedFeatures3D[k].Y;
                            trackedFeatures3Dm[k, 2] = trackedFeatures3D[k].Z;

                            tf3D[k, 0] = trackedFeatures3D[k].X;
                            tf3D[k, 1] = trackedFeatures3D[k].Y;
                            tf3D[k, 2] = trackedFeatures3D[k].Z;
                        }

                        var eigenvectors = new Mat();
                        var mean = new Mat();
                        CvInvoke.PCACompute(trackedFeatures3Dm, mean, eigenvectors);
                        var eigenvectorsMatrix = new Matrix<double>(eigenvectors.Rows, eigenvectors.Cols, eigenvectors.DataPointer);

                        CollectionAssert.AreEqual(eigenvectorsMatrix.Data, eigenvectorsCplusPlus.Data, matrixComparer);

                        // Step 2. Subtract the mean
                        double[] meanArr = tf3D.Mean(0);
                        double[,] dataAdjustArr = tf3D.Subtract(meanArr, 0);

                        var method = PrincipalComponentMethod.Center;
                        var pca = new PrincipalComponentAnalysis(method);
                        pca.Learn(tf3D.ToJagged());

                        var meanMatrix = new Matrix<double>(mean.Rows, mean.Cols, mean.DataPointer);
                        CollectionAssert.AreEqual(meanMatrix.Data.ToJagged()[0], meanArr, matrixComparer);

                        int numInliers = 0;
                        var normalOfPlane = eigenvectorsMatrix.GetRow(2).ToUMat().ToMat(AccessType.Fast);
                        CvInvoke.Normalize(normalOfPlane, normalOfPlane);

                        var normalOfPlaneMatrix = new Matrix<double>(normalOfPlane.Rows, normalOfPlane.Cols, normalOfPlane.DataPointer);
                        var normalOfPlaneArray = new[] { normalOfPlaneMatrix[0, 0], normalOfPlaneMatrix[0, 1], normalOfPlaneMatrix[0, 2] };

                        CollectionAssert.AreEqual(normalOfPlaneArray, normalOfPlaneCplusPlus, matrixComparer);

                        double pToPlaneThresh = Math.Sqrt(pca.Eigenvalues.ElementAt(2));

                        Assert.AreEqual(pToPlaneTrashCplusPlus, pToPlaneThresh, 0.2);

                        var statusArray = new byte[trackedFeatures3D.Size];
                        for (int k = 0; k < trackedFeatures3D.Size; k++)
                        {
                            var t1 = new double[] { trackedFeatures3D[k].X, trackedFeatures3D[k].Y, trackedFeatures3D[k].Z };
                            var t2 = t1.Subtract(meanArr);
                            var w = new Matrix<double>(new[,] { { t2[0], t2[1], t2[2] } });
                            double d = Math.Abs(normalOfPlane.Dot(w));
                            if (d < pToPlaneThresh)
                            {
                                numInliers++;
                                statusArray[k] = 1;
                            }
                        }

                        Assert.AreEqual(numInliersCplusPlus, numInliers);

                        var statusVector = new VectorOfByte(statusArray);
                        CollectionAssert.AreEqual(statusArrCplusPlus.ToArray(), statusVector.ToArray());

                        var bootstrapping = numInliers / (double)trackedFeatures3D.Size < 0.75;
                        if (!bootstrapping)
                        {
                            trackedFeatures3DMCplusPlus = Getmatrix($"I = {i}txt - trackedFeatures3DM.txt");

                            //enough features are coplanar, keep them and flatten them on the XY plane
                            Utils.KeepVectorsByStatus(ref trackedFeatures, ref trackedFeatures3D, statusVector);

                            //the PCA has the major axes of the plane
                            var projected = new Mat();
                            CvInvoke.PCAProject(trackedFeatures3Dm, mean, eigenvectors, projected);
                            var projectedMatrix = new Matrix<double>(projected.Rows, projected.Cols, projected.DataPointer);

                            projectedTrackedFeaturesCplusPlus = Getmatrix($"I = {i}txt - projected trackedFeatures3mat.txt");
                            CollectionAssert.AreEqual(projectedTrackedFeaturesCplusPlus.Data, projectedMatrix.Data, matrixComparer);

                            projectedMatrix.GetCol(2).SetValue(0);
                            projectedMatrix.CopyTo(trackedFeatures3Dm);

                            CollectionAssert.AreEqual(trackedFeatures3DMCplusPlus.Data, trackedFeatures3Dm.Data, matrixComparer);
                        }
                        else
                        {
                            //cerr << "not enough features are coplanar" << "\n";
                            bootstrapKp = bootstrapKpOrig;
                            trackedFeatures = trackedFeaturesOrig;
                        }
                    }

                    //return true;
                }

                currentGray = capture.QueryFrame();
                CvInvoke.CvtColor(currentGray, currentGray, ColorConversion.Bgr2Gray);
            }
        }

        #region HelperMethods

        private VectorOfPointF GetPoints(string name)
        {
            var points = new List<PointF>();

            using (
               var reader = File.OpenText(
                       $@"{TestCaseTestProjectPath}\csharplogs\{name}"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Remove(line.Length - 1, 1);
                    if (line.Contains("["))
                        line = line.Remove(0, 1);

                    if (line.Contains(";"))
                        line = line.Replace(";", "");

                    var items = line.Split(',');
                    float x = float.Parse(items[0]);
                    float y = float.Parse(items[1]);
                    var point = new PointF(x, y);
                    points.Add(point);
                }
            }

            return new VectorOfPointF(points.ToArray());
        }

        private VectorOfPoint3D32F GetPoints3d(string name)
        {
            var points = new List<MCvPoint3D32f>();

            using (
               var reader = File.OpenText(
                       $@"{TestCaseTestProjectPath}\csharplogs\{name}"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Remove(line.Length - 1, 1);
                    if (line.Contains("["))
                        line = line.Remove(0, 1);

                    if (line.Contains(";"))
                        line = line.Replace(";", "");

                    var items = line.Split(',');
                    float x = float.Parse(items[0]);
                    float y = float.Parse(items[1]);
                    float z = float.Parse(items[2]);
                    var point = new MCvPoint3D32f(x, y, z);
                    points.Add(point);
                }
            }

            return new VectorOfPoint3D32F(points.ToArray());
        }

        private Matrix<double> Getmatrix3X3(string name)
        {
            var matrix = new Matrix<double>(3, 3);

            int i = 0;

            using (
               var reader = File.OpenText(
                       $@"{TestCaseTestProjectPath}\csharplogs\{name}"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Remove(line.Length - 1, 1);
                    if (line.Contains("["))
                        line = line.Remove(0, 1);

                    if (line.Contains("]"))
                        line = line.Remove(line.Length - 1, 1);

                    if (line.Contains(";"))
                        line = line.Replace(";", "");

                    var items = line.Split(',');
                    double x1 = double.Parse(items[0]);
                    double x2 = double.Parse(items[1]);
                    double x3 = double.Parse(items[2]);

                    matrix[i, 0] = x1;
                    matrix[i, 1] = x2;
                    matrix[i, 2] = x3;

                    i++;
                }
            }

            return matrix;
        }

        private VectorOfByte GetByteVector(string name)
        {
            var list = new List<byte>();

            int i = 0;

            using (
               var reader = File.OpenText(
                       $@"{TestCaseTestProjectPath}\csharplogs\{name}"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Remove(line.Length - 1, 1);
                    if (line.Contains("["))
                        line = line.Remove(0, 1);

                    if (line.Contains("]"))
                        line = line.Remove(line.Length - 1, 1);

                    line = line.Replace(";", "");

                    if (line.Contains(",") && line.Length > 4)
                    {
                        var items = line.Split(',');
                        foreach (var item in items)
                        {
                            byte itemNumber = (byte)int.Parse(item);
                            list.Add(itemNumber);
                        }
                    }
                    else
                    {
                        byte number = (byte)int.Parse(line);
                        list.Add(number);
                    }

                    i++;
                }
            }

            return new VectorOfByte(list.ToArray());
        }

        private double[] GetDoubleArray(string name)
        {
            var list = new List<double>();

            int i = 0;

            using (
               var reader = File.OpenText(
                       $@"{TestCaseTestProjectPath}\csharplogs\{name}"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Remove(line.Length - 1, 1);
                    if (line.Contains("["))
                        line = line.Remove(0, 1);

                    if (line.Contains("]"))
                        line = line.Remove(line.Length - 1, 1);

                    line = line.Replace(";", "");

                    var lines = line.Split(',');
                    foreach (var item in lines)
                    {
                        double number = double.Parse(item);
                        list.Add(number);
                    }

                    i++;
                }
            }

            return list.ToArray();
        }

        private double GetDouble(string name)
        {
            string line;
            using (
               var reader = File.OpenText(
                       $@"{TestCaseTestProjectPath}\csharplogs\{name}"))
            {
                line = reader.ReadToEnd();
            }

            return Convert.ToDouble(line);
        }

        private int GetInt(string name)
        {
            string line;
            using (
               var reader = File.OpenText(
                       $@"{TestCaseTestProjectPath}\csharplogs\{name}"))
            {
                line = reader.ReadToEnd();
            }

            return Convert.ToInt32(line);
        }

        private Matrix<double> Getmatrix(string name)
        {
            var list = new List<List<double>>();

            using (
               var reader = File.OpenText(
                       $@"{TestCaseTestProjectPath}\csharplogs\{name}"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Remove(line.Length - 1, 1);
                    if (line.Contains("["))
                        line = line.Remove(0, 1);

                    if (line.Contains("]"))
                        line = line.Remove(line.Length - 1, 1);

                    if (line.Contains(";"))
                        line = line.Replace(";", "");

                    var items = line.Split(',');
                    double x1 = double.Parse(items[0]);
                    double x2 = double.Parse(items[1]);
                    double x3 = double.Parse(items[2]);

                    list.Add(new List<double> { x1, x2, x3 });
                }
            }

            var matrix = new Matrix<double>(list.Count, 3);

            int i = 0;
            foreach (var row in list)
            {
                matrix[i, 0] = row[0];
                matrix[i, 1] = row[1];
                matrix[i, 2] = row[2];

                i++;
            }

            return matrix;
        }

        #endregion
    }
}
