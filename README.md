# 🏃‍♂️ BodyCheck - RPG Action Game

유니티(Unity)로 구현한 **2D RPG 액션 게임** 샘플 프로젝트입니다. 
플레이어가 다양한 장비와 스킬을 통해 몬스터를 처치하는 로그라이크 던전 시스템을 핵심으로 하며, **확장성 있는 아키텍처와 메모리 최적화**에 중점을 두어 설계했습니다.

## 🎮 게임 특징
- **던전/필드 시스템**: 게임 모드별 상속 기반 아키텍처
- **스탯 시스템**: 기본/가변/조건부 스탯 3단계 계산
- **ObjectPool 활용**: 메모리 최적화된 몬스터/이펙트 관리
- **Addressable Asset**: 비동기 리소스 로딩

---

## 📌 **핵심 코드 (포트폴리오 포인트)**

### 1️⃣ 확장성 높은 Object Pooling Pattern
📄 [ObjectPool.cs](Assets/Game/Script/Pool/ObjectPool.cs)
* **포인트:** 반복적인 Instantiate/Destroy를 방지하여 CPU 부하 및 GC 발생 최소화.
* **특징:** 큐(Queue) 기반의 범용 설계로 다양한 오브젝트에 즉시 적용 가능.

```csharp
// 재사용 가능하고 확장성 높은 풀 시스템
public GameObject GetObject()
{
    if(this.poolingObjectQueue.Count<=1) 
        poolingObjectQueue.Enqueue(this.CreateNewObject());
    var obj = this.poolingObjectQueue.Dequeue();
    obj.transform.SetParent(null);
    obj.gameObject.SetActive(true);
    return obj;
}

2️⃣ Addressable Asset 기반 리소스 관리
📄 Assets/Game/Script/Manager/Assets.cs
* 포인트: 비동기 로딩을 통해 로딩 화면 및 인게임 프레임 드랍 방지.
* 특징: 메모리 관리 자동화를 위한 커스텀 컴포넌트 결합.
public static void CreateAsset<T>(string path, Action<T> callback, Transform parent = null)
{
    Addressables.InstantiateAsync(path, parent).Completed += (handle) => {
        handle.Result.AddComponent<AddressableAsset>();
        callback.Invoke(handle.Result.GetComponent<T>());
    };
}

3️⃣ 안전한 Singleton & JSON Data System
📄 Assets/Game/Script/Manager/Data.cs
* 포인트: 런타임 중 데이터 유실 방지 및 전역 접근성 확보.
* 특징: 10초 주기 자동 저장 및 Newtonsoft.Json 활용.
public static Data I
{
    get
    {
        if (i == null)
        {
            i = FindObjectOfType<Data>();
            if (i == null)
            {
                GameObject singleton = new GameObject(typeof(Data).ToString());
                i = singleton.AddComponent<Data>();
                DontDestroyOnLoad(singleton);
            }
        }
        return i;
    }
}
포인트: 디자인 패턴 이해도, 안전한 구현

4️⃣ OOP 기반 3단계 Stat System
📄 Assets/Game/Script/Actor/Actor.cs
* 포인트: 복잡한 스탯 계산 로직을 프로퍼티 내 캡슐화.
* 설명: 기본/가변/조건부 스탯을 분리하여 버프/디버프 시스템 확장이 용이함.
public Stat MStat { get => stat+vStat; } // 기본+가변
public Stat NMStat { get => stat+vStat+cStat; } // 모든 스탯 합산
public Stat NStat { // 현재 최대치 (자동 계산)
    get => nStat+cStat;
    set {
        hp += (int)(value.hp-nStat.hp);
        nStat = value;
        actorRoot.SetAttackSpeed(NStat.attackSpeed);
    }
}
포인트: 복잡한 로직을 프로퍼티로 처리, OOP 설계

🏗️ 아키텍처
* GameMode Base: Field와 Dungeon 모드로 상속 확장하여 모드별 독립적 로직 구현.
* Actor Base: Player와 Monster의 공통 로직을 추상화하여 중복 코드 제거.

📊 주요 시스템
게임 매니저: 게임 상태 관리, 레벨별 난이도 조정
데이터 매니저: JSON 저장/로드, 자동 저장 (10초마다)
리소스 매니저: Addressable Assets으로 비동기 로딩
오브젝트 풀: 몬스터/이펙트/텍스트 풀링으로 성능 최적화
스탯 시스템: 기본/가변/조건부 3단계 스탯 계산

🛠️ 기술 스택
Engine: Unity 2022+
Language: C#
Save System: JSON (Newtonsoft.Json)
Resource Loading: Addressable Assets
UI/VFX: DOTween, Particle System


역할: 전체 시스템 설계 및 구현
