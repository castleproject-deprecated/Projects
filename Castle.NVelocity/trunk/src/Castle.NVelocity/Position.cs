// Copyright 2007 Jonathon Rossi - http://www.jonorossi.com/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.NVelocity
{
    using System;

    public class Position : IComparable<Position>
    {
        private int startLine;
        private int startPos;
        private int endLine;
        private int endPos;

        public Position()
        {
        }

        public Position(int startLine, int startPos)
        {
            this.startLine = startLine;
            this.startPos = startPos;
        }

        public Position(int startLine, int startPos, int endLine, int endPos)
        {
            this.startLine = startLine;
            this.startPos = startPos;
            this.endLine = endLine;
            this.endPos = endPos;
        }

        public int StartLine
        {
            get { return startLine; }
            set { startLine = value; }
        }

        public int StartPos
        {
            get { return startPos; }
            set { startPos = value; }
        }

        public int EndLine
        {
            get { return endLine; }
            set { endLine = value; }
        }

        public int EndPos
        {
            get { return endPos; }
            set { endPos = value; }
        }

        public bool Equals(Position anotherPos)
        {
            return (anotherPos.StartLine == startLine) &&
                (anotherPos.StartPos == startPos) &&
                (anotherPos.EndLine == endLine) &&
                (anotherPos.EndPos == endPos);
        }

        public int CompareTo(Position other)
        {
            int lineDif = startLine - other.StartLine;
            if (lineDif == 0)
                return startPos - other.StartPos;
            else
                return lineDif;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}",
                startLine, startPos, endLine, endPos);
        }
    }
}
