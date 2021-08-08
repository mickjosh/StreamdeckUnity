using System;
using UnityEngine;

namespace StudioVR.Streamdeck
{
    public static class StreamdeckTextureExtension
    {
        /// <summary>
        /// Convert a texture to a base64 string with a mime header
        /// </summary>
        /// <param name="tex">The texture to put in base64 string</param>
        /// <param name="format">The format to encode the texture into</param>
        /// <returns>The texture in a base64 string</returns>
        public static string ToBase64String(this Texture2D tex, ImageFormat format = ImageFormat.jpg)
        {
            return $"data:image/{format.ToString()};base64,{Convert.ToBase64String(tex.EncodeToJPG())}";
        }
    }

    public enum ImageFormat
    {
        jpg,
        png
    }
}
