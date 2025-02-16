using System;

[Serializable]
public class OID : IEquatable<OID>
{
    public int ObjectID;
    public int VariantID;

    public OID(int objectID, int variantID)
    {
        ObjectID = objectID;
        VariantID = variantID;
    }
    
    public bool Equals(OID other)
    {
        return ObjectID == other.ObjectID && VariantID == other.VariantID;
    }

    public override bool Equals(object obj)
    {
        return obj is OID other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ObjectID, VariantID);
    }
}