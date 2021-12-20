using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Molinos.Scato.Web.Helpers
{
    public class ZPLConveter {
        private int blackLimit = 380;
        private int total;
        private int widthBytes;
        private bool compressHex = false;

        private static Dictionary<int, String> mapCode = new Dictionary<int, String>
        {
            {1, "G"},
            {2, "H"},
            {3, "I"},
            {4, "J"},
            {5, "K"},
            {6, "L"},
            {7, "M"},
            {8, "N"},
            {9, "O"},
            {10, "P"},
            {11, "Q"},
            {12, "R"},
            {13, "S"},
            {14, "T"},
            {15, "U"},
            {16, "V"},
            {17, "W"},
            {18, "X"},
            {19, "Y"},
            {20, "g"},
            {40, "h"},
            {60, "i"},
            {80, "j"},
            {100, "k"},
            {120, "l"},
            {140, "m"},
            {160, "n"},
            {180, "o"},
            {200, "p"},
            {220, "q"},
            {240, "r"},
            {260, "s"},
            {280, "t"},
            {300, "u"},
            {320, "v"},
            {340, "w"},
            {360, "x"},
            {380, "y"},
            {400, "z"}
        };

        public string convertfromImg(Image  image) {
            String cuerpo = createBody(new Bitmap(image));
            if (compressHex)
            {
                cuerpo = encodeHexAscii(cuerpo);
            }

            return headDoc() + cuerpo + footDoc();        
        }

        private String createBody(Bitmap orginalImage) {
            StringBuilder sb = new StringBuilder();
            Graphics graphics = Graphics.FromImage(orginalImage);
            graphics.DrawImage(orginalImage, 0, 0);
            int height = orginalImage.Height;
            int width = orginalImage.Width;
            int red, green, blue, index=0;        
            char[] auxBinaryChar =  {'0', '0', '0', '0', '0', '0', '0', '0'};
            widthBytes = width/8;
            if(width%8>0){
                widthBytes= (((int)(width/8))+1);
            } else {
                widthBytes= width/8;
            }
            this.total = widthBytes*height;
            for (int h = 0; h<height; h++)
            {
                for (int w = 0; w<width; w++)
                {
                    var rgb = orginalImage.GetPixel(w, h);
                    red = rgb.R;
                    green = rgb.G;
                    blue = rgb.B;
                    char auxChar = '1';
                    int totalColor = red + green + blue;
                    if(totalColor>blackLimit){
                        auxChar = '0';
                    }
                    auxBinaryChar[index] = auxChar;
                    index++;
                    if(index==8 || w==(width-1))
                    {
                        sb.Append(fourByteBinary(new String(auxBinaryChar)));
                        auxBinaryChar =  new char[]{'0', '0', '0', '0', '0', '0', '0', '0'};
                        index=0;
                    }
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }
        private String fourByteBinary(String binaryStr){
            int dec = Convert.ToInt32(binaryStr,2);
            if (dec>15){
                return Convert.ToString(dec,16).ToUpper();
            } else {
                return "0" + Convert.ToString(dec,16).ToUpper();
            }
        }
        private String encodeHexAscii(String code){
            int maxlinea =  widthBytes * 2;        
            StringBuilder sbCode = new StringBuilder();
            StringBuilder sbLinea = new StringBuilder();
            String previousLine = null;
            int counter = 1;
            char aux = code[0];
            bool firstChar = false; 
            for(int i = 1; i< code.Length; i++ ){
                if(firstChar){
                    aux = code[i];
                    firstChar = false;
                    continue;
                }
                if(code[i]=='\n'){
                    if(counter>=maxlinea && aux=='0'){
                        sbLinea.Append(",");
                    } else if(counter>=maxlinea && aux=='F'){
                        sbLinea.Append("!");
                    } else if (counter>20){
                        int multi20 = (counter/20)*20;
                        int resto20 = (counter%20);
                        sbLinea.Append(mapCode[multi20]);
                        if(resto20!=0){
                            sbLinea.Append(mapCode[resto20] + aux);    
                        } else {
                            sbLinea.Append(aux);    
                        }
                    } else {
                        sbLinea.Append(mapCode[counter] + aux);
                        if(mapCode[counter]==null){
                        }
                    }
                    counter = 1;
                    firstChar = true;
                    if(sbLinea.ToString() == previousLine){
                        sbCode.Append(":");
                    } else {
                        sbCode.Append(sbLinea.ToString());
                    }                
                    previousLine = sbLinea.ToString();
                    sbLinea.Clear();
                    continue;
                }
                if(aux == code[i]){
                    counter++;                
                } else {
                    if(counter>20){
                        int multi20 = (counter/20)*20;
                        int resto20 = (counter%20);
                        sbLinea.Append(mapCode[multi20]);
                        if(resto20!=0){
                            sbLinea.Append(mapCode[resto20] + aux);    
                        } else {
                            sbLinea.Append(aux);    
                        }
                    } else {
                        sbLinea.Append(mapCode[counter] + aux);
                    }
                    counter = 1;
                    aux = code[i];
                }            
            }
            return sbCode.ToString();
        }
        private String headDoc(){
            //"^XA " +
            String str = "^FO{0},{1}^GFA,"+ total + ","+ total + "," + widthBytes +", ";
            return str;
        }
        private String footDoc()
        {
            String str = "^FS"; //+ "^XZ";        
            return str;
        }
        public void setCompressHex(bool compressHex) {
            this.compressHex = compressHex;
        }
        public void setBlacknessLimitPercentage(int percentage){
            blackLimit = (percentage * 768 / 100);
        }
    }
    
}