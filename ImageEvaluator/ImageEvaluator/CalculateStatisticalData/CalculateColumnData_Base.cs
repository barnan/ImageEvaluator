﻿using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace ImageEvaluator.CalculateStatisticalData
{
    abstract class CalculateColumnData_Base : IColumnDataCalculator
    {

        protected bool _initialized;
        protected int _width;
        protected int _height;
        protected Image<Gray, float> _meanVector;
        protected Image<Gray, float> _stdVector;
        protected float[,,] _resultVector1;
        protected float[,,] _resultVector2;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public CalculateColumnData_Base(int width, int height)
        {
            _width = width;
            _height = height;

            Init();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <param name="meanVector"></param>
        /// <param name="stdVector"></param>
        /// <returns></returns>
        public abstract bool CalculateStatistics(Image<Gray, float> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, ref Image<Gray, float> meanVector, ref Image<Gray, float> stdVector);



        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="maskImage"></param>
        /// <returns></returns>
        protected virtual bool CheckInputData(Image<Gray, float> inputImage, Image<Gray, byte> maskImage, int[,] pointArray, Image<Gray, float> meanVector, Image<Gray, float> stdVector)
        {
            if (inputImage == null || inputImage.Height < 0 || inputImage.Height > 10000 || inputImage.Width < 0 || inputImage.Width > 10000)
            {
                return false;
            }
            if (meanVector == null || stdVector == null || meanVector.Height != inputImage.Height || stdVector.Height != inputImage.Height)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool Init()
        {
            if (_initialized)
                return true;

            try
            {
                _meanVector = new Image<Gray, float>(_width, _height);
                _stdVector = new Image<Gray, float>(_width, _height);

                _resultVector1 = _meanVector.Data;
                _resultVector2 = _stdVector.Data;

                return _initialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during CalculcateColumnData - Init: {ex.Message}.");
                return _initialized = false;
            }

        }




    }

}
