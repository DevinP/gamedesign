/*To obtain a bounding box around our model in model space (the space the model is created in i.e. with 0,0,0 normally at the centre of the model in your art 
 * package) we need to go through all the vertices in the model keeping a track of the minimum and maximum x, y and z positions. 
 * This gives us two corners of the box from which all the other corners can be calculated since the box is aligned along the axis 
 * (hence it is known as an Axis Aligned Bounding Box or AABB for short).
 * Since each model is made from a number of mesh we need to calculate minimum and maximum values from the vertex positions for each mesh. 
 * The ModelMesh object in XNA is split into parts which in turn provides access to the buffer holding the vertex data (VertexBuffer) 
 * from which we can obtain a copy of the vertices using the GetData call. 
 * (Note that the reason a mesh has parts is because it may use differerent materials per part but for bounding box creation we are not bothered about that 
 * so we look through all parts in the mesh).
 * We can access the vertex data via the GetData call however we have no way of knowing what type of data it is. 
 * This is because a vertex can have a number of elements like position, colour, normal, texture coordinate etc. 
 * The good thing though is that we can find out the size in bytes of one vertex (the stride) and we also know that every vertex has the position element 
 * specified first. We can therefore extract the position by grabbing the first 3 floats per vertex and using the stride value to advance to the next one.
 * /


/*public BoundingBox CalculateBoundingBox()
{

// Create variables to hold min and max xyz values for the model. Initialise them to extremes
Vector3 modelMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
Vector3 modelMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

foreach (ModelMesh mesh in m_model.Meshes)
{
  //Create variables to hold min and max xyz values for the mesh. Initialise them to extremes
   Vector3 meshMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
   Vector3 meshMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

  // There may be multiple parts in a mesh (different materials etc.) so loop through each
  foreach (ModelMeshPart part in mesh.MeshParts)
   {
     // The stride is how big, in bytes, one vertex is in the vertex buffer
     // We have to use this as we do not know the make up of the vertex
     int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

     byte[] vertexData = new byte[stride * part.NumVertices];
     part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, 1); // fixed 13/4/11

     // Find minimum and maximum xyz values for this mesh part
     // We know the position will always be the first 3 float values of the vertex data
     Vector3 vertPosition=new Vector3();
     for (int ndx = 0; ndx < vertexData.Length; ndx += stride)
      {
         vertPosition.X= BitConverter.ToSingle(vertexData, ndx);
         vertPosition.Y = BitConverter.ToSingle(vertexData, ndx + sizeof(float));
         vertPosition.Z= BitConverter.ToSingle(vertexData, ndx + sizeof(float)*2);

         // update our running values from this vertex
         meshMin = Vector3.Min(meshMin, vertPosition);
         meshMax = Vector3.Max(meshMax, vertPosition);
     }
   }

   // transform by mesh bone transforms
   meshMin = Vector3.Transform(meshMin, m_transforms[mesh.ParentBone.Index]);
   meshMax = Vector3.Transform(meshMax, m_transforms[mesh.ParentBone.Index]);

   // Expand model extents by the ones from this mesh
   modelMin = Vector3.Min(modelMin, meshMin);
   modelMax = Vector3.Max(modelMax, meshMax);
}


// Create and return the model bounding box
return new BoundingBox(modelMin, modelMax);

}*/

/*Note that after calculating the minimum and maximum vertex position values for the mesh we also need to take into account any bone transformations 
 * that may need to be applied. This can be seen in the code above where the bounding box just calculated for a mesh is transformed by the bone matrix.
 * For more accurate collisions you may also want to store the individual mesh bounding boxes. 
 * A first collision check would then check the model bounding box and if this proved true go through each mesh bounding box. 
 * This would also have the benefit of giving you the part of the model where the collision occurred.
 * There is one issue with the above code though which is that we are not sure that every vertex in the vertex buffer is used by the mesh. 
 * You might think it would naturally however it is not guaranteed. This could be solved by using the index buffer to look up the vertices.
 * Ultimately you may wish to create a custom content processor as by writing code that hooks into the actual model loading we can deal with the 
 * vertex data prior to it being put into a vertex buffer and optimised and reorganised by XNA.
*/

/*It takes some time to go through all the vertices finding min and max values therefore you should only do this once at model load time 
 * and not during the game loop. For maximum speed you would do this in advance of the game running.
*/