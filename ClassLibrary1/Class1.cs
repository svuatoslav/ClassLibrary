using System;
using System.Collections.Generic;
using System.Linq;
using MazeInterface;

[assembly: Identification(Name = "Москаленко Нитаева", Group = "М80-102М-22")]
namespace ClassLibrary1
{
    public class Class1 : IMaze
    {
        private int Width;
        private int Height;
        MazeCell[] TheMaze;
        private List<int> path = new List<int>();
        public Class1(int w, int h)
        {
            TheMaze = new MazeCell[w * h];
            Width = w; Height = h;
        }
        public List<int> Path { get => path; }
        public MazeCell this[int ind]
        {
            get
            {
                if (ind < 0 || ind >= TheMaze.Length)
                    throw new IndexOutOfRangeException("за предел");
                return TheMaze[ind];
            }
            set
            {
                if (ind < 0 || ind >= TheMaze.Length)
                    throw new IndexOutOfRangeException("за предел");
                TheMaze[ind] = value;
            }
        }
        public int Length { get => TheMaze.Length; }
        public void Generate()
        {
            Random random = new Random();
            path = FindPath(random);
            for (int i = 0; i < TheMaze.Length; i++)
            {
                if (path.Contains(i))
                {
                    if (i == path.First())
                    {
                        TheMaze[i].cellType = CellType.Enter;
                        TheMaze[i].left = false;
                        if (!isUp(i) && path[1] == Up(i))
                            TheMaze[i].up = false;
                        else
                            TheMaze[i].up = true;
                    }
                    else if (i == path.Last())
                    {
                        TheMaze[i].cellType = CellType.Exit;
                        if (!isUp(i) && path[path.Count - 2] == Up(i))
                            TheMaze[i].up = false;
                        else
                            TheMaze[i].up = true;
                        if(!isLeft(i) && path[path.Count - 2] == Left(i))
                            TheMaze[i].left = false;
                        else
                            TheMaze[i].left = true;
                    }
                    else
                    {
                        TheMaze[i].cellType = CellType.Inside;
                        if (isLeft(i))
                            TheMaze[i].left = true;
                        if (isUp(i))
                            TheMaze[i].up = true;
                        if (!TheMaze[i].left || !TheMaze[i].up)
                        {
                            bool up = true;
                            bool left = true;
                            for (int j = path.IndexOf(i); j < path.Count; j++)
                                if (path[j] == i)
                                {
                                    if (!TheMaze[i].left && left)
                                        if (path[j - 1] == i - 1 || path[j + 1] == i - 1)
                                            left = false;//нет стенки
                                    if (!TheMaze[i].up && up)
                                        if (path[j - 1] == i - Width || path[j + 1] == i - Width)
                                            up = false;//нет стенки
                                }
                            if (up)
                                TheMaze[i].up = true;
                            if (left)
                                TheMaze[i].left = true;
                        }
                    }
                }
                else
                {
                    TheMaze[i].cellType = CellType.Inside;
                    if (isLeft(i))
                        TheMaze[i].left = true;
                    if (isUp(i))
                        TheMaze[i].up = true;
                    if (!TheMaze[i].left)
                        if (random.Next(100) > 50)
                            TheMaze[i].left = true;
                    if (!TheMaze[i].up)
                        if (random.Next(100) > 50)
                            TheMaze[i].up = true;
                }
            }
        }
        private List<int> FindPath(Random random)
        {
            path.Add((random.Next(Height) - 1) * Width);
            int exit = random.Next(Height) * Width - 1;
            Direction direction = Direction.Left;
            Direction NewDirection = RandDirection(random);
            for (int k = 0; k <= Height * Width * Height * Width; k++)
            {
                NewDirection = ChoosingDirection(direction, NewDirection, random);
                direction = NewDirection;
                for (int i, j = random.Next(1, Math.Min(Height, Width) + 1); j != 0; j--)
                {
                    if (ComparisonDirections(direction, Direction.Left))
                        i = Left(path.Last());
                    else if (ComparisonDirections(direction, Direction.Up))
                        i = Up(path.Last());
                    else if (ComparisonDirections(direction, Direction.Down))
                        i = Down(path.Last());
                    else if (ComparisonDirections(direction, Direction.Right))
                        i = Right(path.Last());
                    else
                        throw new Exception("Что-то не так с направлением");
                    if (i == -1)
                        break;
                    if (i == exit)
                    {
                        path.Add(i);
                        return path;
                    }
                    path.Add(i);
                }
            }
            throw new Exception("Ошибка в генерации пути, много итераций");
        }
        private Direction RandDirection(Random random) => (Direction)typeof(Direction).GetEnumValues().GetValue(random.Next(typeof(Direction).GetEnumValues().Length));
        private bool ComparisonDirections(Direction Copy, Direction Original) => Copy.ToString().StartsWith(Original.ToString(), StringComparison.CurrentCultureIgnoreCase);
        private Direction ChoosingDirection(Direction direction, Direction NewDirection, Random random)
        {
            while (true)
            {
                if (direction == NewDirection)
                    NewDirection = RandDirection(random);
                else if (ComparisonDirections(direction, Direction.Left) && ComparisonDirections(NewDirection, Direction.Right))
                    NewDirection = RandDirection(random);
                else if (ComparisonDirections(direction, Direction.Right) && ComparisonDirections(NewDirection, Direction.Left))
                    NewDirection = RandDirection(random);
                else if (ComparisonDirections(direction, Direction.Up) && ComparisonDirections(NewDirection, Direction.Down))
                    NewDirection = RandDirection(random);
                else if (ComparisonDirections(direction, Direction.Down) && ComparisonDirections(NewDirection, Direction.Up))
                    NewDirection = RandDirection(random);
                else if (isLeft(path.Last()))
                {
                    if (isUp(path.Last()))
                    {
                        if (ComparisonDirections(direction, Direction.Left))
                            NewDirection = Direction.Down;
                        else if (ComparisonDirections(direction, Direction.Up))
                            NewDirection = Direction.Right;
                        else
                            throw new Exception("Ошибка в направлении isLeft->isUp");
                        return NewDirection;
                    }
                    else if (isDown(path.Last()))
                    {
                        if (ComparisonDirections(direction, Direction.Left))
                            NewDirection = Direction.Up;
                        else if(ComparisonDirections(direction, Direction.Down))
                            NewDirection = Direction.Right;
                        else
                            throw new Exception("Ошибка в направлении isLeft->isDown");
                        return NewDirection;
                    }
                    else if (ComparisonDirections(NewDirection, Direction.Left))
                        NewDirection = RandDirection(random);
                    else return NewDirection;
                }
                else if (isUp(path.Last()))
                {
                    if (isLeft(path.Last()))
                    {
                        if (ComparisonDirections(direction, Direction.Up))
                            NewDirection = Direction.Right;
                        else if (ComparisonDirections(direction, Direction.Left))
                            NewDirection = Direction.Down;
                        else
                            throw new Exception("Ошибка в направлении isUp->isLeft");
                        return NewDirection;
                    }
                    else if (isRight(path.Last()))
                    {
                        if (ComparisonDirections(direction, Direction.Up))
                            NewDirection = Direction.Left;
                        else if (ComparisonDirections(direction, Direction.Right))
                            NewDirection = Direction.Down;
                        else
                            throw new Exception("Ошибка в направлении isUp->isRight");
                        return NewDirection;
                    }
                    else if (ComparisonDirections(NewDirection, Direction.Up))
                        NewDirection = RandDirection(random);
                    else return NewDirection;
                }
                else if (isDown(path.Last()))
                {
                    if (isLeft(path.Last()))
                    {
                        if (ComparisonDirections(direction, Direction.Down))
                            NewDirection = Direction.Right;
                        else if (ComparisonDirections(direction, Direction.Left))
                            NewDirection = Direction.Up;
                        else
                            throw new Exception("Ошибка в направлении isDown->isLeft");
                        return NewDirection;
                    }
                    else if (isRight(path.Last()))
                    {
                        if (ComparisonDirections(direction, Direction.Down))
                            NewDirection = Direction.Left;
                        else if (ComparisonDirections(direction, Direction.Right))
                            NewDirection = Direction.Up;
                        else
                            throw new Exception("Ошибка в направлении isDown->isRight");
                        return NewDirection;
                    }
                    else if (ComparisonDirections(NewDirection, Direction.Down))
                        NewDirection = RandDirection(random);
                    else return NewDirection;
                }
                else if (isRight(path.Last()))
                {
                    if (isUp(path.Last()))
                    {
                        if (ComparisonDirections(direction, Direction.Right))
                            NewDirection = Direction.Down;
                        else if(ComparisonDirections(direction, Direction.Up))
                            NewDirection = Direction.Left;
                        else
                            throw new Exception("Ошибка в направлении isRight->isUp");
                        return NewDirection;
                    }
                    else if (isDown(path.Last()))
                    {
                        if (ComparisonDirections(direction, Direction.Right))
                            NewDirection = Direction.Up;
                        else if(ComparisonDirections(direction, Direction.Down))
                            NewDirection = Direction.Left;
                        else
                            throw new Exception("Ошибка в направлении isRight->isDown");
                        return NewDirection;
                    }
                    else if (ComparisonDirections(NewDirection, Direction.Right))
                        NewDirection = RandDirection(random);
                    else return NewDirection;
                }
                else return NewDirection;
            }
        }
        public bool isUp(int i) => i < Width;
        public bool isDown(int i) => i >= (Height - 1) * Width;
        public bool isRight(int i) => i % Width == Width - 1;
        public bool isLeft(int i) => i % Width == 0;
        public int Left(int i)
        {
            if (isLeft(i)) 
                return -1;
            else
                return i - 1;
        }
        public int Right(int i)
        {
            if (isRight(i)) 
                return -1;
            else
                return i + 1;
        }
        public int Up(int i)
        {
            if (isUp(i)) 
                return -1;
            else
                return i - Width;
        }
        public int Down(int i)
        {
            if (isDown(i)) 
                return -1;
            else
                return i + Width;
        }
        private enum Direction
        {
            Left,
            Up,
            Up1,
            Down,
            Down1,
            Right,
            Right1,
            Right2,
        }
    }
}
