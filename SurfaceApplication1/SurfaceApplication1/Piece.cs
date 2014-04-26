using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DigitalFauvel
{

    public class Pieces : Dictionary<string, Piece>
    {
        public Piece GetPieceById(string id)
        {
            return base[id];
        }
    }
    
    public class Piece
    {
        public string ID { get; private set; }
        public string Title { get; private set; }
        public int Voices { get; private set; }
        public string OriginalText { get; private set; }
        public string ModernFrench { get; private set; }
        public string English { get; private set; }

        public Piece(string id)
        {
            ID = id;
        }

        public Piece(XElement xPiece)
        {
            ID = xPiece.Attribute("id").Value.Replace("_t","");
            Title = xPiece.Descendants("title").First().Value;
            if (ID.Contains("Mo"))
            {
                Voices = Convert.ToInt32(xPiece.Descendants("nv").First().Value);
            }
            else
            {
                Voices = 1;
            }
            ModernFrench = Search.getByTag(ID, SurfaceWindow1.modFrXml);
            English = Search.getByTag(ID, SurfaceWindow1.engXml);
        }
    }
}
