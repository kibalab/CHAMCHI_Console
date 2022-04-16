![그룹 11](https://user-images.githubusercontent.com/31209389/162854013-bea8905c-fcba-4499-8406-762b7b29ff89.png)
<p align="center">
  <a href="https://booth.pm/ko/items/3779891">
  <img width="15%" src="https://user-images.githubusercontent.com/31209389/162854080-7c9a03d4-d1ba-4c7a-aab2-ba92b8a356e7.png" />
  </a>
</p>

<h1 align="center">
  :fish: CHAMCHI (TUNA) Debug Console:fish:<br>
</h1>
<p align="center">
VRChat UdonSharp 개발용 디버그 콘솔<br>
Debug console for VRChat UdonSharp development<br>
</p><br>
<br>

## :star: Introdution / 자기소개
 - **Ingame Logging**
 - **Synchronize Logs**
<br>

## :package: Require / 필요
[VRChat](https://store.steampowered.com/app/438100/VRChat/)<br>
[Unity 2019.4](https://unity3d.com/kr/unity/whats-new/2019.4.31)<br>
[VRCSDK 3.0](https://vrchat.com/home/download)<br>
[UDONSharp](https://github.com/MerlinVR/UdonSharp)<br>
<br>

## :tools: Use / 사용

### :open_file_folder: Install
- Please download it here -> **[Release](https://github.com/kibalab/CHAMCHI_Console/releases)**
- Import it into your Unity Project
- Place the "Toggle" prefab
<br>

### :ballot_box: Logger Method Parameter
```CSharp
Debug.Log(<UdonSharpBehaviour Class>, <String Message>);
Debug.LogWarn(<UdonSharpBehaviour Class>, <String Message>);
Debug.LogError(<UdonSharpBehaviour Class>, <String Message>);
```
<br>

### :page_facing_up: Example
```CSharp
public class MyClass
{
  public logPanel Debug;
  
  public void MyFunction()
  {
    Debug.Log(this, "Hello, World!");
    Debug.LogWarn(this, "Hello, World!");
    Debug.LogError(this, "Hello, World!");
  }
}
```
<br>

### :level_slider: Inspector Editor
<p align="center">
  <img width="50%" src="https://user-images.githubusercontent.com/31209389/162856572-66939b3b-7d02-4902-8e66-1b4d017e78f8.png" />
</p>
<br>

#### :electric_plug: Field Auto Setter
<p align="center">
  <img width="40%" src="https://user-images.githubusercontent.com/31209389/162856150-64858806-bb20-4371-8a84-23c826271205.png" />
</p>

- Auto Setter 버튼을 눌러 모든 UdonSharpBehaviours에 자동으로 할당<br>
- Automatically assign to all UdonSharpBehaviours by pressing the Auto Setter button<br>
<br>

### 🖧 User Network Logging
<p align="center">
  <img width="80%" src="https://user-images.githubusercontent.com/31209389/162856433-6b547204-7433-40ce-8279-c316d8ae3fca.png" />
</p>

- 타인의 로그를 열람 가능함<br>
- Can view other people's logs<br><br>
  <br>
  
## Developer / 개발 참여
### [Seyrinn](https://github.com/seyrinn)
* 🎲 System Dev<br>
* 📓 Project Manage<br>
* 🖧 Networking<br>
* ✅ Q/A<br>
### [KIBA_](https://github.com/kibalab)
* 🏗 UI Design<br>
* 🎨 Resource Design<br>
* 🎲 System Design<br>
* 📓 Project Manage<br>
* 🖧 Networking<br>

<br><br>

## OLD Project / 구 프로젝트
https://github.com/kibalab/K13A_Udon_Console

## 라이센스 / License

**MIT License**