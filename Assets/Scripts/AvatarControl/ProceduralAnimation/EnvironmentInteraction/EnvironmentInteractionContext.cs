
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnvironmentInteractionContext
{
    private TwoBoneIKConstraint leftFootIKConstraint;
    private TwoBoneIKConstraint righFootIKConstraint;
    private MultiRotationConstraint leftFootRotConstraint;
    private MultiRotationConstraint rightFootRotConstraint;
    private BoxCollider leftFootCollider;
    private BoxCollider rightFootCollider;

    public EnvironmentInteractionContext(TwoBoneIKConstraint leftFootIKConstraint, TwoBoneIKConstraint righFootIKConstraint, MultiRotationConstraint leftFootRotConstraint, MultiRotationConstraint rightFootRotConstraint, BoxCollider leftFootCollider, BoxCollider rightFootCollider)
    {
        this.leftFootIKConstraint = leftFootIKConstraint;
        this.righFootIKConstraint = righFootIKConstraint;
        this.leftFootRotConstraint = leftFootRotConstraint;
        this.rightFootRotConstraint = rightFootRotConstraint;
        this.leftFootCollider = leftFootCollider;
        this.rightFootCollider = rightFootCollider;
    }

    // getter methods
    public TwoBoneIKConstraint LeftFootIKConstraint => leftFootIKConstraint;
    public TwoBoneIKConstraint RighFootIKConstraint => righFootIKConstraint;
    public MultiRotationConstraint LeftFootRotConstraint => leftFootRotConstraint;
    public MultiRotationConstraint RightFootRotConstraint => rightFootRotConstraint;
    public BoxCollider LeftFootCollider => leftFootCollider;
    public BoxCollider RightFootCollider => rightFootCollider;

}
