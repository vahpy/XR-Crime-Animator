# XR Crime Animator
This repository provides source code for Criminator, an Easy-to-Use XR "Crime Animator", published at CHI 2026. See [Citation](#citation) for details.

# Requirements

- Unity 2022.3.x (This project has been tested by Unity 2022.3.29f1 and 2022.3.62f3)
- Meta Quest headset (Quest 2, Quest Pro, Quest 3, Quest 3S)
- Meta Air Link enabled
- Windows PC with VR-ready GPU

# Getting Started
Follow the steps below to import packages and prepare the project to run on a headset:

- Unzip the repository on your PC
- Add the project to Unity Hub
- Open the project using Unity 2022.3.x
- If prompted, allow Unity to upgrade to your current editor version
- Unity should automatically install required packages. If not:
  -	Go to Window > Package Manager
  -	Install any missing or unimported packages
- If prompted to restart the editor to switch the Input System, accept and restart Unity
- Navigate to Assets/Scenes
- Open StudyScene
- If prompted to import TMP Pro, import TMP Essentials and Examples and Extras. 
- Turn on your Meta Quest headset
- Connect via Meta Air Link to the PC
- Press Play in the Unity Editor

# Disclaimer
This source code is provided for **academic and research purposes only**. It is shared to support transparency and reproducibility of the associated academic work.

The software is provided “as is”, without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose, and noninfringement. In no event shall the authors or affiliated institutions be liable for any claim, damages, or other liability arising from, out of, or in connection with the software or the use or other dealings in the software.

Use of this code is **at your own risk**.

# License

Please refer to [License](LICENSE) for information regarding this source code, and [Notice](Notice.md) for information on third-party packages and artefacts.

# Citation
If you use this code, please cite the associated paper:
```code
@inproceedings{Pooryousef2026,
  author = {Pooryousef, Vahid and Besan\c{c}on, Lonni and Cordeil, Maxime and Flight,
            Chris and Ross AM, Alastair M and Bassed, Richard and Dwyer, Tim},
  title = {Criminator: An Easy-to-Use XR "Crime Animator" for Rapid Reconstruction and Analysis of Dynamic Crime Scenes},
  year = {2026},
  publisher = {Association for Computing Machinery},
  address = {New York, NY, USA},
  doi = {10.1145/3772318.3791210},
  booktitle = {Proceedings of the 2026 CHI Conference on Human Factors in Computing Systems},
  location = {Barcelona, Spain},
  series = {CHI '26}
}
```