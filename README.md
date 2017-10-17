# Mixed Reality Unity Implementation

This is a general purpose implementation to realize Mixed Reality in Unity.
For a detailed look at the implementation you can [follow the blog entry on the
artcom development blog][mr-blog].

To get started, you will need OSVR/SteamVR, setup a scene with VR enabled, add
the "Cameras" prefab to the tracking object and the MR-Root inside the SteamVR
box.

This solution needs an input webcam signal - if you prefer your own camera
solution, we'd suggest to use a converter from your video signal to webcam.

[mr-blog]: http://artcom.github.io/mixed-reality/