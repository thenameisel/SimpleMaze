using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMaze
{
    class MapLoader
    {
        ////list of map locations in file, I do want to put this in a config file along with the enum at some point
        //var map02 = Path.Combine(Content.RootDirectory, "map02.csv");
        //var map02Col = Path.Combine(Content.RootDirectory, "mapCol02.csv");

        //map variables
        public Dictionary<Vector2, int> tileMap { get; }
        public Dictionary<Vector2, int> tileMapCol { get; }

        //tile size
        static int _enviroSize = 64;

        //location data for the tiles in the enviromentatl texture
        private List<Rectangle> _textureStore = new()
        {
       
            //floor tiles
            new Rectangle(0, 0, _enviroSize, _enviroSize),
            new Rectangle(_enviroSize, 0, _enviroSize, _enviroSize),
            new Rectangle(_enviroSize*2, 0, _enviroSize, _enviroSize),
            new Rectangle(_enviroSize*3, 0, _enviroSize, _enviroSize),
            new Rectangle(0, _enviroSize, _enviroSize, _enviroSize),
            new Rectangle(),
            new Rectangle(),
            new Rectangle(),
            //wall tiles
            new Rectangle(0, _enviroSize*2, _enviroSize, _enviroSize),
            new Rectangle(_enviroSize, _enviroSize*2, _enviroSize, _enviroSize),
            new Rectangle(),
            new Rectangle(),
            //misc tiles
            new Rectangle(0, _enviroSize*3, _enviroSize, _enviroSize),
            new Rectangle(),
            new Rectangle(),
            new Rectangle(),
        };

        public List<Rectangle> textureStore
        {
            get { return _textureStore; }
            
        }

        //constructor
        public MapLoader(Enum LEVEL)
        {
            tileMap = new Dictionary<Vector2, int>();
            tileMapCol = new Dictionary<Vector2, int>();

            switch (LEVEL)
            {
                case Level.LEVEL1:
                    tileMap = LoadMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "maps", "map01.csv"));
                    tileMapCol = LoadMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "maps", "mapCol01.csv"));
                    break;
                case Level.LEVEL2:
                    tileMap = LoadMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "maps", "map02.csv"));
                    tileMapCol = LoadMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "maps", "mapCol02.csv"));
                    break;
                case Level.LEVEL3:
                    tileMap = LoadMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "maps", "map03.csv"));
                    tileMapCol = LoadMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "maps", "mapCol03.csv"));
                    break;
            }



        }

        //method to load a map and return a dictionary of Dictionary<Vector2, int>
        private Dictionary<Vector2, int> LoadMap(string filepath)
        {
            Dictionary<Vector2, int> result = new();

            StreamReader reader = new(filepath);

            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');

                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value))
                    {
                        result[new Vector2(x, y)] = value;
                    }
                }
                y++;
            }
            return result;
        }


    }
}
