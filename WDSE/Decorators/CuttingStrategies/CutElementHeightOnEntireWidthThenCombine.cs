using System.Drawing;
using ImageMagick;
using OpenQA.Selenium;
using WDSE.Helpers;
using WDSE.Interfaces;

namespace WDSE.Decorators.CuttingStrategies
{
    public class CutElementHeightOnEntireWidthThenCombine : ICuttingStrategy
    {
        private readonly By _elementByToCut;

        /// <summary>
        /// </summary>
        /// <param name="by">How to find element.</param>
        public CutElementHeightOnEntireWidthThenCombine(By by)
        {
            _elementByToCut = by;
        }

        public IMagickImage Cut(IWebDriver driver, IMagickImage magickImage)
        {
            var elementCoordinates = driver.GetElementCoordinates(_elementByToCut);
            if (!driver.IsElementPartialInViewPort(elementCoordinates.y, elementCoordinates.bottom)) return magickImage;
            var width = magickImage.Width;
            var height = magickImage.Height;
            using (var collection = new MagickImageCollection())
            {
                var heightT = 0 + elementCoordinates.y;
                if (heightT < 0 && height < elementCoordinates.bottom) return null;
                if (heightT < 0) heightT = elementCoordinates.bottom;

                var firstPart = elementCoordinates.y <= 0
                    ? null
                    : new MagickImage(MagickColor.FromRgb(255, 255, 255), width, (uint)heightT);
                var secondPart = elementCoordinates.bottom > height
                    ? null
                    : new MagickImage(MagickColor.FromRgb(255, 255, 255), width, (uint)(magickImage.Height - elementCoordinates.y - elementCoordinates.height));
                if (firstPart != null)
                {
                    collection.Add(firstPart);
                }

                if (secondPart != null)
                {
                    collection.Add(secondPart);
                }

                var overAllImage = collection.Count == 0 ? null : collection.AppendVertically();
                return overAllImage == null ? null : new MagickImage(overAllImage);
            }
        }
    }
}