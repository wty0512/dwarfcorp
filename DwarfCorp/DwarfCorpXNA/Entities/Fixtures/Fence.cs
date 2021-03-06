// WorkPile.cs
// 
//  Modified MIT License (MIT)
//  
//  Copyright (c) 2015 Completely Fair Games Ltd.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// The following content pieces are considered PROPRIETARY and may not be used
// in any derivative works, commercial or non commercial, without explicit 
// written permission from Completely Fair Games:
// 
// * Images (sprites, textures, etc.)
// * 3D Models
// * Sound Effects
// * Music
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DwarfCorp.GameStates;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace DwarfCorp
{
    public class Fence : Fixture
    {
        public float FenceRotation { get; set; }

        public Fence()
        {
            
        }

        public Fence(ComponentManager componentManager, Vector3 position, float orientation, string asset) :
            base(componentManager, position, new SpriteSheet(asset, 32, 32), new Point(0, 0))
        {
            FenceRotation = orientation;
            GetComponent<SimpleSprite>().OrientationType = SimpleSprite.OrientMode.Fixed;
            GetComponent<SimpleSprite>().LocalTransform = Matrix.CreateRotationY(FenceRotation);
        }


        public override void CreateCosmeticChildren(ComponentManager manager)
        {
            base.CreateCosmeticChildren(manager);
            GetComponent<SimpleSprite>().OrientationType = SimpleSprite.OrientMode.Fixed;
            GetComponent<SimpleSprite>().LocalTransform = Matrix.CreateRotationY(FenceRotation);
        }

        private struct FenceSegmentInfo
        {
            public GlobalVoxelOffset VoxelOffset;
            public Vector3 VisibleOffset;
            public float Angle;
        }

        private static FenceSegmentInfo[] FenceSegments = new FenceSegmentInfo[]
        {
            new FenceSegmentInfo
            {
                VoxelOffset = new GlobalVoxelOffset(0,0,1),
                VisibleOffset = new Microsoft.Xna.Framework.Vector3(0,0,0.45f),
                Angle = (float)Math.Atan2(0,1)
            },

            new FenceSegmentInfo
            {
                VoxelOffset = new GlobalVoxelOffset(0,0,-1),
                VisibleOffset = new Microsoft.Xna.Framework.Vector3(0,0,-0.45f),
                Angle = (float)Math.Atan2(0,-1)
            },

            new FenceSegmentInfo
            {
                VoxelOffset = new GlobalVoxelOffset(1,0,0),
                VisibleOffset = new Microsoft.Xna.Framework.Vector3(0.45f,0,0),
                Angle = (float)Math.Atan2(1,0)
            },

            new FenceSegmentInfo
            {
                VoxelOffset = new GlobalVoxelOffset(-1,0,0),
                VisibleOffset = new Microsoft.Xna.Framework.Vector3(-0.45f,0,0),
                Angle = (float)Math.Atan2(-1,0)
            },
        };

        public static IEnumerable<Body> CreateFences(
            ComponentManager components,
            string asset, 
            IEnumerable<VoxelHandle> Voxels, 
            bool createWorkPiles)
        {
            Vector3 off = (Vector3.One * 0.5f) + Vector3.Up;
            foreach (var voxel in Voxels)
            {
                foreach (var segment in FenceSegments)
                {
                    var neighbor = VoxelHelpers.GetNeighbor(voxel, segment.VoxelOffset);
                    if (neighbor.IsValid && !Voxels.Any(v => v == neighbor))
                        yield return new Fence(components,
                            voxel.WorldPosition + off + segment.VisibleOffset,
                            segment.Angle, asset);
                }

                if (createWorkPiles && MathFunctions.RandEvent(0.1f))
                {
                    yield return new WorkPile(components, voxel.WorldPosition + off);
                }
            }
        }
    }
}
