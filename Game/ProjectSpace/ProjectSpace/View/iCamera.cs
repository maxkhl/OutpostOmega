using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OutpostOmega.View
{
    interface iCamera
    {
        Vector3 Position { get; set; }
        Matrix4 GetViewMatrix(Vector3 Lookat);
        Matrix4 ViewProjectionMatrix { get; set; }
        bool LockCursor { get; set; }
        void Update(Scene Scene, Vector3 Lookat);
    }
}
