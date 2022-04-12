![그룹 11](https://user-images.githubusercontent.com/31209389/162854013-bea8905c-fcba-4499-8406-762b7b29ff89.png)
[![그룹 2](https://user-images.githubusercontent.com/31209389/162854080-7c9a03d4-d1ba-4c7a-aab2-ba92b8a356e7.png)](https://booth.pm/ko/items/3779891)
[![그룹 2](https://user-images.githubusercontent.com/31209389/162855182-0022e7c9-c747-437a-ad5c-d34da509a007.png)](https://github.com/vrchat-community/UdonSharp)


# CHAMCHI (TUNA) Debug Console
VRChat UdonSharp 개발용 디버그 콘솔<br>
Debug console for VRChat UdonSharp development<br>
  
## Require / 필요
[VRChat](https://store.steampowered.com/app/438100/VRChat/)<br>
[Unity 2018.4](https://unity3d.com/kr/unity/whats-new/2018.4.20)<br>
[VRCSDK 3.0](https://vrchat.com/home/download)<br>
[UDONSharp](https://github.com/MerlinVR/UdonSharp)<br>

## Use / 사용

### Logger Method Parameter
```
Debug.Log(<UdonSharpBehaviour Class>, <String Message>);
Debug.LogWarn(<UdonSharpBehaviour Class>, <String Message>);
Debug.LogError(<UdonSharpBehaviour Class>, <String Message>);
```

### Example
```
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

### Inspector Editor
![레이어 17](https://user-images.githubusercontent.com/31209389/162856572-66939b3b-7d02-4902-8e66-1b4d017e78f8.png)<br>

#### Field Auto Setter
![image](https://user-images.githubusercontent.com/31209389/162856150-64858806-bb20-4371-8a84-23c826271205.png)<br>

Auto Setter 버튼을 눌러 모든 UdonSharpBehaviours에 자동으로 할당<br>
Automatically assign to all UdonSharpBehaviours by pressing the Auto Setter button<br>

### User Network Logging
![레이어 16](https://user-images.githubusercontent.com/31209389/162856433-6b547204-7433-40ce-8279-c316d8ae3fca.png)

타인의 로그를 열람 가능함<br>
Can view other people's logs<br>
  
## Developer / 개발 참여
### [Seyrinn](https://github.com/seyrinn)
* 🎲 System Dev<br>
* 📓 Project Manage<br>
* 🖧 Networking<br>
* ✅ Q/A<br>
### [KIBA_](https://github.com/kibalab)
* 🏗 UI Design<br>
* 🎨 Resource Design<br>
* 🎲 System Dev<br>
* 📓 Project Manage<br>
* 🖧 Networking<br>

## OLD Project / 구 프로젝트
https://github.com/kibalab/K13A_Udon_Console

## 라이센스 / License

MIT License
