using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEvaluator.SearchContourPoints
{
    class BorderSearcher_CSharp1 : BorderSearcher_Base
    {

        internal BorderSearcher_CSharp1(int border)
        {
            _borderSkipSize = border;
        }


        protected override void CalculatePoints(Image<Gray, byte> maskImage)
        {
            try
            {
                byte[,,] imgArray = maskImage.Data;

                for (int i = 0; i < maskImage.Height; i++)
                {
                    for (int j = 0; j < maskImage.Width / 4; j++)
                    {
                        if (imgArray[i, j, 0] > 0)
                        {
                            _borderPoints[i, 0] = j + _borderSkipSize;
                            break;
                        }
                    }
                }

                for (int i = 0; i < maskImage.Height; i++)
                {
                    for (int j = maskImage.Width - 1; j > maskImage.Width * 3 / 4; j--)
                    {
                        if (imgArray[i, j, 0] > 0)
                        {
                            _borderPoints[i, 1] = j - _borderSkipSize;
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Exception caught in BorderSearcher_CSharp1-CalculatePoints: {ex.Message}.");
            }

        }
    }




    /// <summary>
    /// 
    /// </summary>
    class Factory_BorderSearcher_CSharp1 : IBorderSeracher_Creator
    {
        public IBorderSearcher Factory(int border, bool showImages)
        {
            throw new NotImplementedException();
        }

    }
}

