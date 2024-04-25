# Character Stats

### 소개
RPG 게임에서 사용하는 케릭터 스탯 관리 코드 입니다. 케릭터의 스탯 이외에도 기본값과 변화를 분리하여 처리해야 하는 부분에 사용할 수 있습니다.

### 설치방법
1. 패키지 관리자의 툴바에서 좌측 상단에 플러스 메뉴를 클릭합니다.
2. 추가 메뉴에서 Add package from git URL을 선택하면 텍스트 상자와 Add 버튼이 나타납니다.
3. https://github.com/DarkNaku/CharacterStats.git 입력하고 Add를 클릭합니다.

### 사용방법

샘플제작중...

### 클래스 

### `Stat<T>`

직렬화 불가

#### 속성

##### `public float InitialValue { get; }`

설명: 생성자를 통해 최초 설정된 값을 가지고 있음. (읽기전용)

타입: float 

##### `public float BaseValue { get; set; }`

설명: 수정 사항들이 반영 되지 않은 값을 가지고 있음.

타입: float 

##### `public float Value { get; }`

설명: 모든 수정 사항들을 적용한 최종 결과값. (읽기전용)

타입: float 

##### `public T Key { get; }`

설명: 스탯을 고유하게 만들기 위해 사용하는 키값. 일반적으로 string 또는 enum 타입을 지정합니다. (읽기전용)

타입: 제네릭 

##### `public UnityEvent<Stat<T>> OnChangeValue { get; }`

설명: 스탯 수정 사항이 발생하는 경우 이벤트를 받기위해 사용합니다. (읽기전용)

타입: UnityEvent

#### 생성자

##### `public Stat(T key, float initialValue)`

설명: 초기값과 Key 값을 설정할 수 있습니다.

매개변수:
* `key` (T): 키값.
* `initialValue` (float): 초기값.

##### `public Stat(Stat<T> parent)`

설명: 다른 스탯을 참조하여 초기 값으로 사용할 수 있습니다. 참조한 스탯이 변경되는 경우 기반으로 생성한 스탯도 값이 변경됩니다. 전투에서 버프나 디버프 같은 일시적인 스탯 수치와 분리해야 할 때 유용합니다.

매개변수:
* `parent` (Stat<T>): 참조 스탯.

#### 함수

##### `public void Add(Modifier modifier)`

설명: 수정 사항을 추가합니다.

매개변수:
* `modifier` (Modifier): 수정 사항.

Returns: void

##### `public void Remove(Modifier modifier)`

설명: 수정 사항을 제거합니다.

매개변수:
* `modifier` (Modifier): 수정 사항.

Returns: void

##### `public void RemoveByID(string id)`

설명: ID가 같은 모든 수정 사항을 찾아 제거합니다.

매개변수:
* `id` (string): 수정 사항 ID.

Returns: void

##### `public void RemoveBySource(object source)`

설명: object를 소스로 사용하고 있는 모든 수정 사항을 찾아 제거합니다.

매개변수:
* `source` (object): 수정 사항을 부여한 주체.

Returns: void

##### `public IReadOnlyList<Modifier> GetModifiers(ModifierType modifierType)`

설명: modifierType의 모든 수정사항을 얻어옵니다.

매개변수:
* `modifierType` (ModifierType): 수정 사항 타입.

Returns: IReadOnlyList<Modifier>

### `Modifier`

직렬화 가능

#### 속성

##### `public Modifier Type { get; }`

설명: 수정 타입. 계산되는 방식을 지정한다. (읽기전용)

타입: Modifier

##### `public float Value { get; }`

설명: 수정에 적용할 수치. (읽기전용)

타입: float

##### `public string ID { get; }`

설명: 수정사항을 고유하게 구분하기 위한 문자열.

타입: string

##### `public object Source { get; set; }`

설명: 수정 사항을 부여한 주체 등 수정사항들을 그룹화 하기 위한 속성. (주의 : 이 속성은 직렬화 되지 않습니다.)

타입: object 

#### 생성자

##### `public Modifier(ModifierType type, float value)`

설명: 수정 타입과 수정 수치를 설정하고 이 후 수정은 불가능 합니다.

매개변수:
* `type` (ModifierType): 수정타입.
* `value` (float): 수치.

#### 함수

##### `public Modifier SetID(string id)`

설명: 수정사항 들의 그룹을 만들기 위해 ID를 설정합니다. 수정 사항의 제거가 보다 편해집니다.

매개변수:
* `id` (string): ID.

Returns: void


### `CharacterStats<T>`

#### 속성

##### `public string Name { get;}`

설명: 케릭터 이름.

타입: string

##### `public IReadOnlyList<IStat> Stats {get}`

설명: 모든 스탯 정보.

타입: IReadOnlyList<IStat>

##### `public IReadOnlyDictionary<T, Stat<T>> All { get; }`

설명: 스탯 타입을 포함한 모든 스탯 정보.

타입: IReadOnlyDictionary<T, Stat<T>>

##### `public UnityEvent<CharacterStats<T>, Stat<T>> OnChangeStat { get; }`

설명: 수정 사항의 변동에 대한 이벤트.

타입: UnityEvent<CharacterStats<T>, Stat<T>>

##### `public Stat<T> this[T key] { get; }`

설명: Key를 통해 특정 Stat을 얻어 올 수 있습니다.

타입: Stat<T>

#### 함수

##### `public bool Contains(T key)`

설명: Key 보유 여부 확인.

매개변수:
* `key` (T): 스탯의 키.

Returns: bool

##### `public bool AddStat(T key, float initialValue)`

설명: 스탯을 추가 합니다.

매개변수:
* `key` (T): 스탯의 키.
* `initialiValue` (float): 스탯 초기값.

Returns: bool 

##### `public bool AddModifier(T key, Modifier modifier)`

설명: 수정 사항을 추가합니다.

매개변수:
* `key` (T): 스탯의 키.
* `modifier` (Modifier): 수정사항.

Returns: bool 

##### `public void RemoveModifier(T key, Modifier modifier)`

설명: 수정 사항을 제거합니다.

매개변수:
* `key` (T): 스탯의 키.
* `modifier` (Modifier): 수정사항.

Returns: void

##### `public void RemoveModifierByID(string id)`

설명: 모든 Stat에서 같은 id를 가진 모든 수정을 제거합니다.

매개변수:
* `id` (string): 문자열 ID

Returns: void

##### `public void RemoveModifierBySource(object source)`

설명: 모든 Stat에서 같은 소스를 가진 모든 수정을 제거합니다.

매개변수:
* `source` (object): 소스

Returns: void

##### `public void Log(string title)`

설명: 스탯들의 상태를 로그로 출력하는 함수입니다.

매개변수:
* `title` (string): 로그 구분을 위한 타이틀 문자열.

Returns: void