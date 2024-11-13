using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Hellmade.Sound
{
    /// <summary>
    /// 오디오와 소리를 재생하고 관리하는 역할을 하는 정적 클래스입니다.
    /// </summary>
    public class EazySoundManager : MonoBehaviour
    {
        /// <summary>
        /// 사운드 매니저가 부착된 게임오브젝트
        /// </summary>
        public static GameObject Gameobject { get { return Instance.gameObject; } }

        /// <summary>
        /// true로 설정하면 동일한 오디오 클립을 가진 새로운 음악 오디오는 무시됩니다.
        /// </summary>
        public static bool IgnoreDuplicateMusic { get; set; }

        /// <summary>
        /// true로 설정하면 동일한 오디오 클립을 가진 새로운 사운드 오디오는 무시됩니다.
        /// </summary>
        public static bool IgnoreDuplicateSounds { get; set; }

        /// <summary>
        /// true로 설정하면 동일한 오디오 클립을 가진 새로운 UI 사운드 오디오는 무시됩니다.
        /// </summary>
        public static bool IgnoreDuplicateUISounds { get; set; }

        /// <summary>
        /// 글로벌 볼륨
        /// </summary>
        public static float GlobalVolume { get; set; }

        /// <summary>
        /// 글로벌 음악 볼륨
        /// </summary>
        public static float GlobalMusicVolume { get; set; }

        /// <summary>
        /// 글로벌 사운드 볼륨
        /// </summary>
        public static float GlobalSoundsVolume { get; set; }

        /// <summary>
        /// 글로벌 UI 사운드 볼륨
        /// </summary>
        public static float GlobalUISoundsVolume { get; set; }

        private static EazySoundManager instance = null;

        private static Dictionary<int, Audio> musicAudio;
        private static Dictionary<int, Audio> soundsAudio;
        private static Dictionary<int, Audio> UISoundsAudio;
        private static Dictionary<int, Audio> audioPool;

        private static bool initialized = false;

        private static EazySoundManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (EazySoundManager)FindObjectOfType(typeof(EazySoundManager));
                    if (instance == null)
                    {
                        // 게임오브젝트 생성 및 컴포넌트 추가
                        instance = (new GameObject("EazySoundManager")).AddComponent<EazySoundManager>();
                    }
                }
                return instance;
            }
        }

        static EazySoundManager()
        {
            Instance.Init();
        }

        /// <summary>
        /// 사운드 매니저를 초기화합니다.
        /// </summary>
        private void Init()
        {
            if (!initialized)
            {
                musicAudio = new Dictionary<int, Audio>();
                soundsAudio = new Dictionary<int, Audio>();
                UISoundsAudio = new Dictionary<int, Audio>();
                audioPool = new Dictionary<int, Audio>();

                GlobalVolume = 1;
                GlobalMusicVolume = 1;
                GlobalSoundsVolume = 1;
                GlobalUISoundsVolume = 1;

                IgnoreDuplicateMusic = false;
                IgnoreDuplicateSounds = false;
                IgnoreDuplicateUISounds = false;

                initialized = true;
                DontDestroyOnLoad(this);
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// 새로운 씬이 로드될 때 발생하는 이벤트
        /// </summary>
        /// <param name="scene">로드된 씬</param>
        /// <param name="mode">씬 로드 모드</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 지속되지 않는 모든 오디오를 중지하고 제거
            RemoveNonPersistAudio(musicAudio);
            RemoveNonPersistAudio(soundsAudio);
            RemoveNonPersistAudio(UISoundsAudio);
        }

        private void Update()
        {
            UpdateAllAudio(musicAudio);
            UpdateAllAudio(soundsAudio);
            UpdateAllAudio(UISoundsAudio);
        }

        /// <summary>
        /// audioType에 따라 오디오 딕셔너리를 가져옵니다.
        /// </summary>
        /// <param name="audioType">반환할 딕셔너리의 오디오 타입</param>
        /// <returns>오디오 딕셔너리</returns>
        private static Dictionary<int, Audio> GetAudioTypeDictionary(Audio.AudioType audioType)
        {
            Dictionary<int, Audio> audioDict = new Dictionary<int, Audio>();
            switch (audioType)
            {
                case Audio.AudioType.Music:
                    audioDict = musicAudio;
                    break;
                case Audio.AudioType.Sound:
                    audioDict = soundsAudio;
                    break;
                case Audio.AudioType.UISound:
                    audioDict = UISoundsAudio;
                    break;
            }

            return audioDict;
        }

        /// <summary>
        /// 지정된 오디오 타입의 IgnoreDuplicates 설정을 가져옵니다.
        /// </summary>
        /// <param name="audioType">반환될 IgnoreDuplicates 설정에 영향을 미치는 오디오 타입</param>
        /// <returns>IgnoreDuplicates 설정 (bool)</returns>
        private static bool GetAudioTypeIgnoreDuplicateSetting(Audio.AudioType audioType)
        {
            switch (audioType)
            {
                case Audio.AudioType.Music:
                    return IgnoreDuplicateMusic;
                case Audio.AudioType.Sound:
                    return IgnoreDuplicateSounds;
                case Audio.AudioType.UISound:
                    return IgnoreDuplicateUISounds;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 오디오 딕셔너리의 모든 오디오 상태를 업데이트합니다.
        /// </summary>
        /// <param name="audioDict">업데이트할 오디오 딕셔너리</param>
        private static void UpdateAllAudio(Dictionary<int, Audio> audioDict)
        {
            // 모든 오디오를 업데이트
            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioDict[key];
                audio.Update();

                // 더 이상 활성화되지 않은(재생 중이지 않은) 오디오는 제거
                if (!audio.IsPlaying && !audio.Paused)
                {
                    Destroy(audio.AudioSource);

                    // 나중에 참조할 수 있도록 오디오 풀에 추가
                    audioPool.Add(key, audio);
                    audio.Pooled = true;
                    audioDict.Remove(key);
                }
            }
        }

        /// <summary>
        /// 오디오 딕셔너리에서 지속되지 않는 모든 오디오를 제거합니다.
        /// </summary>
        /// <param name="audioDict">지속되지 않는 오디오가 제거될 오디오 딕셔너리</param>
        private static void RemoveNonPersistAudio(Dictionary<int, Audio> audioDict)
        {
            // 모든 오디오를 확인하고 씬을 통해 지속되지 않는 경우 제거
            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioDict[key];
                if (!audio.Persist && audio.Activated)
                {
                    Destroy(audio.AudioSource);
                    audioDict.Remove(key);
                }
            }

            // 오디오 풀의 모든 오디오를 확인하고 씬을 통해 지속되지 않는 경우 제거
            keys = new List<int>(audioPool.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioPool[key];
                if (!audio.Persist && audio.Activated)
                {
                    audioPool.Remove(key);
                }
            }
        }

        /// <summary>
        /// 풀에 있는 오디오를 복원하고 해당 오디오 딕셔너리에 다시 추가합니다.
        /// </summary>
        /// <param name="audioType">복원할 오디오의 오디오 타입</param>
        /// <param name="audioID">복원될 오디오의 ID</param>
        /// <returns>오디오가 복원되면 true, 오디오가 오디오 풀에 없으면 false</returns>
        public static bool RestoreAudioFromPool(Audio.AudioType audioType, int audioID)
        {
            if (audioPool.ContainsKey(audioID))
            {
                Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);
                audioDict.Add(audioID, audioPool[audioID]);
                audioPool.Remove(audioID);

                return true;
            }

            return false;
        }

        #region GetAudio Functions

        /// <summary>
        /// audioID를 가진 오디오를 반환합니다. 해당 오디오를 찾을 수 없는 경우 null을 반환합니다.
        /// </summary>
        /// <param name="audioID">검색할 오디오의 ID</param>
        /// <returns>audioID를 가진 오디오, 해당 오디오를 찾을 수 없는 경우 null</returns>
        public static Audio GetAudio(int audioID)
        {
            Audio audio;

            audio = GetMusicAudio(audioID);
            if (audio != null)
            {
                return audio;
            }

            audio = GetSoundAudio(audioID);
            if (audio != null)
            {
                return audio;
            }

            audio = GetUISoundAudio(audioID);
            if (audio != null)
            {
                return audio;
            }

            return null;
        }

        /// <summary>
        /// 지정된 audioClip을 재생하는 첫 번째 오디오를 반환합니다. 해당 오디오를 찾을 수 없는 경우 null을 반환합니다.
        /// </summary>
        /// <param name="audioClip">검색할 오디오 클립</param>
        /// <returns>audioClip을 재생하는 첫 번째 오디오, 해당 오디오를 찾을 수 없는 경우 null</returns>
        public static Audio GetAudio(AudioClip audioClip)
        {
            Audio audio = GetMusicAudio(audioClip);
            if (audio != null)
            {
                return audio;
            }

            audio = GetSoundAudio(audioClip);
            if (audio != null)
            {
                return audio;
            }

            audio = GetUISoundAudio(audioClip);
            if (audio != null)
            {
                return audio;
            }

            return null;
        }

        /// <summary>
        /// audioID를 가진 음악 오디오를 반환합니다. 해당 오디오를 찾을 수 없는 경우 null을 반환합니다.
        /// </summary>
        /// <param name="audioID">반환될 음악 오디오의 ID</param>
        /// <returns>audioID를 가진 음악 오디오, 해당 오디오를 찾을 수 없는 경우 null</returns>
        public static Audio GetMusicAudio(int audioID)
        {
            return GetAudio(Audio.AudioType.Music, true, audioID);
        }

        /// <summary>
        /// 지정된 audioClip을 재생하는 첫 번째 음악 오디오를 반환합니다. 해당 오디오를 찾을 수 없는 경우 null을 반환합니다.
        /// </summary>
        /// <param name="audioClip">검색할 음악 오디오 클립</param>
        /// <returns>audioClip을 재생하는 첫 번째 음악 오디오, 해당 오디오를 찾을 수 없는 경우 null</returns>
        public static Audio GetMusicAudio(AudioClip audioClip)
        {
            return GetAudio(Audio.AudioType.Music, true, audioClip);
        }

        /// <summary>
        /// audioID를 가진 사운드 fx 오디오를 반환합니다. 해당 오디오를 찾을 수 없는 경우 null을 반환합니다.
        /// </summary>
        /// <param name="audioID">반환될 사운드 fx 오디오의 ID</param>
        /// <returns>audioID를 가진 사운드 fx 오디오, 해당 오디오를 찾을 수 없는 경우 null</returns>
        public static Audio GetSoundAudio(int audioID)
        {
            return GetAudio(Audio.AudioType.Sound, true, audioID);
        }

        /// <summary>
        /// 지정된 audioClip을 재생하는 첫 번째 사운드 오디오를 반환합니다. 해당 오디오를 찾을 수 없는 경우 null을 반환합니다.
        /// </summary>
        /// <param name="audioClip">검색할 사운드 오디오 클립</param>
        /// <returns>audioClip을 재생하는 첫 번째 사운드 오디오, 해당 오디오를 찾을 수 없는 경우 null</returns>
        public static Audio GetSoundAudio(AudioClip audioClip)
        {
            return GetAudio(Audio.AudioType.Sound, true, audioClip);
        }

        /// <summary>
        /// audioID를 가진 UI 사운드 fx 오디오를 반환합니다. 해당 오디오를 찾을 수 없는 경우 null을 반환합니다.
        /// </summary>
        /// <param name="audioID">반환될 UI 사운드 fx 오디오의 ID</param>
        /// <returns>audioID를 가진 UI 사운드 fx 오디오, 해당 오디오를 찾을 수 없는 경우 null</returns>
        public static Audio GetUISoundAudio(int audioID)
        {
            return GetAudio(Audio.AudioType.UISound, true, audioID);
        }

        /// <summary>
        /// 지정된 audioClip을 재생하는 첫 번째 UI 사운드 오디오를 반환합니다. 해당 오디오를 찾을 수 없는 경우 null을 반환합니다.
        /// </summary>
        /// <param name="audioClip">검색할 UI 사운드 오디오 클립</param>
        /// <returns>audioClip을 재생하는 첫 번째 UI 사운드 오디오, 해당 오디오를 찾을 수 없는 경우 null</returns>
        public static Audio GetUISoundAudio(AudioClip audioClip)
        {
            return GetAudio(Audio.AudioType.UISound, true, audioClip);
        }

        private static Audio GetAudio(Audio.AudioType audioType, bool usePool, int audioID)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            if (audioDict.ContainsKey(audioID))
            {
                return audioDict[audioID];
            }

            if (usePool && audioPool.ContainsKey(audioID) && audioPool[audioID].Type == audioType)
            {
                return audioPool[audioID];
            }

            return null;
        }

        private static Audio GetAudio(Audio.AudioType audioType, bool usePool, AudioClip audioClip)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            List<int> audioTypeKeys = new List<int>(audioDict.Keys);
            List<int> poolKeys = new List<int>(audioPool.Keys);
            List<int> keys = usePool ? audioTypeKeys.Concat(poolKeys).ToList() : audioTypeKeys;
            foreach (int key in keys)
            {
                Audio audio = null;
                if (audioDict.ContainsKey(key))
                {
                    audio = audioDict[key];
                }
                else if (audioPool.ContainsKey(key))
                {
                    audio = audioPool[key];
                }
                if (audio == null)
                {
                    return null;
                }
                if (audio.Clip == audioClip && audio.Type == audioType)
                {
                    return audio;
                }
            }

            return null;
        }

        #endregion

        #region Prepare Function

        /// <summary>
        /// 배경 음악을 준비하고 초기화합니다.
        /// </summary>
        /// <param name="clip">준비할 오디오 클립</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PrepareMusic(AudioClip clip)
        {
            return PrepareAudio(Audio.AudioType.Music, clip, 1f, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// 배경 음악을 준비하고 초기화합니다.
        /// </summary>
        /// <param name="clip">준비할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PrepareMusic(AudioClip clip, float volume)
        {
            return PrepareAudio(Audio.AudioType.Music, clip, volume, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// 배경 음악을 준비하고 초기화합니다.
        /// </summary>
        /// <param name="clip">준비할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <param name="loop">음악이 반복되는지 여부</param>
        /// <param name="persist">오디오가 씬 변경 간에 지속되는지 여부</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PrepareMusic(AudioClip clip, float volume, bool loop, bool persist)
        {
            return PrepareAudio(Audio.AudioType.Music, clip, volume, loop, persist, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// 배경 음악을 준비하고 초기화합니다.
        /// </summary>
        /// <param name="clip">준비할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <param name="loop">음악이 반복되는지 여부</param>
        /// <param name="persist">오디오가 씬 변경 간에 지속되는지 여부</param>
        /// <param name="fadeInSeconds">오디오가 페이드 인(볼륨이 높아짐)되는 데 걸리는 시간(초)</param>
        /// <param name="fadeOutSeconds">오디오가 페이드 아웃(볼륨이 낮아짐)되는 데 걸리는 시간(초)</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PrepareMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds)
        {
            return PrepareAudio(Audio.AudioType.Music, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, -1f, null);
        }

        /// <summary>
        /// 배경 음악을 준비하고 초기화합니다.
        /// </summary>
        /// <param name="clip">준비할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <param name="loop">음악이 반복되는지 여부</param>
        /// <param name="persist">오디오가 씬 변경 간에 지속되는지 여부</param>
        /// <param name="fadeInSeconds">오디오가 페이드 인(볼륨이 높아짐)되는 데 걸리는 시간(초)</param>
        /// <param name="fadeOutSeconds">오디오가 페이드 아웃(볼륨이 낮아짐)되는 데 걸리는 시간(초)</param>
        /// <param name="currentMusicfadeOutSeconds">현재 음악 오디오가 페이드 아웃되는 데 걸리는 시간(초). 현재 음악은 자신의 페이드 아웃 시간을 유지합니다. -1이 전달되면 현재 음악은 자신의 페이드 아웃 시간을 유지합니다.</param>
        /// <param name="sourceTransform">음악의 소스가 되는 트랜스폼(3D 오디오가 됩니다). 3D 오디오가 필요하지 않은 경우 null을 사용하세요.</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PrepareMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            return PrepareAudio(Audio.AudioType.Music, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);
        }

        /// <summary>
        /// 사운드 fx를 준비하고 초기화합니다.
        /// </summary>
        /// <param name="clip">준비할 오디오 클립</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PrepareSound(AudioClip clip)
        {
            return PrepareAudio(Audio.AudioType.Sound, clip, 1f, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// 사운드 fx를 준비하고 초기화합니다.
        /// </summary>
        /// <param name="clip">준비할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PrepareSound(AudioClip clip, float volume)
        {
            return PrepareAudio(Audio.AudioType.Sound, clip, volume, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// 사운드 fx를 준비하고 초기화합니다.
        /// </summary>
        /// <param name="clip">준비할 오디오 클립</param>
        /// <param name="loop">사운드가 반복되는지 여부</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PrepareSound(AudioClip clip, bool loop)
        {
            return PrepareAudio(Audio.AudioType.Sound, clip, 1f, loop, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// 사운드 fx를 준비하고 초기화합니다.
        /// </summary>
        /// <param name="clip">준비할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <param name="loop">사운드가 반복되는지 여부</param>
        /// <param name="sourceTransform">사운드의 소스가 되는 트랜스폼(3D 오디오가 됩니다). 3D 오디오가 필요하지 않은 경우 null을 사용하세요.</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PrepareSound(AudioClip clip, float volume, bool loop, Transform sourceTransform)
        {
            return PrepareAudio(Audio.AudioType.Sound, clip, volume, loop, false, 0f, 0f, -1f, sourceTransform);
        }

        /// <summary>
        /// UI 사운드 fx를 준비하고 초기화합니다.
        /// </summary>
        /// <param name="clip">준비할 오디오 클립</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PrepareUISound(AudioClip clip)
        {
            return PrepareAudio(Audio.AudioType.UISound, clip, 1f, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// UI 사운드 fx를 준비하고 초기화합니다.
        /// </summary>
        /// <param name="clip">준비할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PrepareUISound(AudioClip clip, float volume)
        {
            return PrepareAudio(Audio.AudioType.UISound, clip, volume, false, false, 0f, 0f, -1f, null);
        }

        private static int PrepareAudio(Audio.AudioType audioType, AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            if (clip == null)
            {
                Debug.LogError("[Eazy Sound Manager] 오디오 클립이 null입니다.", clip);
            }

            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);
            bool ignoreDuplicateAudio = GetAudioTypeIgnoreDuplicateSetting(audioType);

            if (ignoreDuplicateAudio)
            {
                Audio duplicateAudio = GetAudio(audioType, true, clip);
                if (duplicateAudio != null)
                {
                    return duplicateAudio.AudioID;
                }
            }

            // audioSource 생성
            Audio audio = new Audio(audioType, clip, loop, persist, volume, fadeInSeconds, fadeOutSeconds, sourceTransform);

            // 딕셔너리에 추가
            audioDict.Add(audio.AudioID, audio);

            return audio.AudioID;
        }

        #endregion

        #region Play Functions

        /// <summary>
        /// 배경 음악을 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PlayMusic(AudioClip clip)
        {
            return PlayAudio(Audio.AudioType.Music, clip, 1f, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// 배경 음악을 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PlayMusic(AudioClip clip, float volume)
        {
            return PlayAudio(Audio.AudioType.Music, clip, volume, false, false, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// 배경 음악을 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <param name="loop">음악이 반복되는지 여부</param>
        /// <param name="persist">오디오가 씬 변경 간에 지속되는지 여부</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PlayMusic(AudioClip clip, float volume, bool loop, bool persist)
        {
            return PlayAudio(Audio.AudioType.Music, clip, volume, loop, persist, 1f, 1f, -1f, null);
        }

        /// <summary>
        /// 배경 음악을 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <param name="loop">음악이 반복되는지 여부</param>
        /// <param name="persist">오디오가 씬 변경 간에 지속되는지 여부</param>
        /// <param name="fadeInSeconds">오디오가 페이드 인(볼륨이 높아짐)되는 데 걸리는 시간(초)</param>
        /// <param name="fadeOutSeconds">오디오가 페이드 아웃(볼륨이 낮아짐)되는 데 걸리는 시간(초)</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PlayMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds)
        {
            return PlayAudio(Audio.AudioType.Music, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, -1f, null);
        }

        /// <summary>
        /// 배경 음악을 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <param name="loop">음악이 반복되는지 여부</param>
        /// <param name="persist">오디오가 씬 변경 간에 지속되는지 여부</param>
        /// <param name="fadeInSeconds">오디오가 페이드 인(볼륨이 높아짐)되는 데 걸리는 시간(초)</param>
        /// <param name="fadeOutSeconds">오디오가 페이드 아웃(볼륨이 낮아짐)되는 데 걸리는 시간(초)</param>
        /// <param name="currentMusicfadeOutSeconds">현재 음악 오디오가 페이드 아웃되는 데 걸리는 시간(초). 현재 음악은 자신의 페이드 아웃 시간을 유지합니다. -1이 전달되면 현재 음악은 자신의 페이드 아웃 시간을 유지합니다.</param>
        /// <param name="sourceTransform">음악의 소스가 되는 트랜스폼(3D 오디오가 됩니다). 3D 오디오가 필요하지 않은 경우 null을 사용하세요.</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PlayMusic(AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            return PlayAudio(Audio.AudioType.Music, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);
        }

        /// <summary>
        /// 사운드 fx를 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PlaySound(AudioClip clip)
        {
            return PlayAudio(Audio.AudioType.Sound, clip, 1f, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// 사운드 fx를 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PlaySound(AudioClip clip, float volume)
        {
            return PlayAudio(Audio.AudioType.Sound, clip, volume, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// 사운드 fx를 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="loop">사운드가 반복되는지 여부</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PlaySound(AudioClip clip, bool loop)
        {
            return PlayAudio(Audio.AudioType.Sound, clip, 1f, loop, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// 사운드 fx를 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <param name="loop">사운드가 반복되는지 여부</param>
        /// <param name="sourceTransform">사운드의 소스가 되는 트랜스폼(3D 오디오가 됩니다). 3D 오디오가 필요하지 않은 경우 null을 사용하세요.</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PlaySound(AudioClip clip, float volume, bool loop, Transform sourceTransform)
        {
            return PlayAudio(Audio.AudioType.Sound, clip, volume, loop, false, 0f, 0f, -1f, sourceTransform);
        }

        /// <summary>
        /// UI 사운드 fx를 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PlayUISound(AudioClip clip)
        {
            return PlayAudio(Audio.AudioType.UISound, clip, 1f, false, false, 0f, 0f, -1f, null);
        }

        /// <summary>
        /// UI 사운드 fx를 재생합니다.
        /// </summary>
        /// <param name="clip">재생할 오디오 클립</param>
        /// <param name="volume">음악의 볼륨</param>
        /// <returns>생성된 오디오 객체의 ID</returns>
        public static int PlayUISound(AudioClip clip, float volume)
        {
            return PlayAudio(Audio.AudioType.UISound, clip, volume, false, false, 0f, 0f, -1f, null);
        }

        private static int PlayAudio(Audio.AudioType audioType, AudioClip clip, float volume, bool loop, bool persist, float fadeInSeconds, float fadeOutSeconds, float currentMusicfadeOutSeconds, Transform sourceTransform)
        {
            int audioID = PrepareAudio(audioType, clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds, currentMusicfadeOutSeconds, sourceTransform);

            // 현재 재생 중인 모든 음악 중지
            if (audioType == Audio.AudioType.Music)
            {
                StopAllMusic(currentMusicfadeOutSeconds);
            }

            GetAudio(audioType, false, audioID).Play();

            return audioID;
        }

        #endregion

        #region Stop Functions

        /// <summary>
        /// 모든 오디오 재생 중지
        /// </summary>
        public static void StopAll()
        {
            StopAll(-1f);
        }

        /// <summary>
        /// 모든 오디오 재생 중지
        /// </summary>
        /// <param name="musicFadeOutSeconds">모든 음악 오디오가 페이드 아웃되는 데 걸리는 시간(초). 자신의 페이드 아웃 시간을 유지합니다. -1이 전달되면 모든 음악은 자신의 페이드 아웃 시간을 유지합니다.</param>
        public static void StopAll(float musicFadeOutSeconds)
        {
            StopAllMusic(musicFadeOutSeconds);
            StopAllSounds();
            StopAllUISounds();
        }

        /// <summary>
        /// 모든 음악 재생 중지
        /// </summary>
        public static void StopAllMusic()
        {
            StopAllAudio(Audio.AudioType.Music, -1f);
        }

        /// <summary>
        /// 모든 음악 재생 중지
        /// </summary>
        /// <param name="fadeOutSeconds">모든 음악 오디오가 페이드 아웃되는 데 걸리는 시간(초). 자신의 페이드 아웃 시간을 유지합니다. -1이 전달되면 모든 음악은 자신의 페이드 아웃 시간을 유지합니다.</param>
        public static void StopAllMusic(float fadeOutSeconds)
        {
            StopAllAudio(Audio.AudioType.Music, fadeOutSeconds);
        }

        /// <summary>
        /// 모든 사운드 fx 재생 중지
        /// </summary>
        public static void StopAllSounds()
        {
            StopAllAudio(Audio.AudioType.Sound, -1f);
        }

        /// <summary>
        /// 모든 UI 사운드 fx 재생 중지
        /// </summary>
        public static void StopAllUISounds()
        {
            StopAllAudio(Audio.AudioType.UISound, -1f);
        }

        private static void StopAllAudio(Audio.AudioType audioType, float fadeOutSeconds)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioDict[key];
                if (fadeOutSeconds > 0)
                {
                    audio.FadeOutSeconds = fadeOutSeconds;
                }
                audio.Stop();
            }
        }

        #endregion

        #region Pause Functions

        /// <summary>
        /// 모든 오디오 일시 중지
        /// </summary>
        public static void PauseAll()
        {
            PauseAllMusic();
            PauseAllSounds();
            PauseAllUISounds();
        }

        /// <summary>
        /// 모든 음악 일시 중지
        /// </summary>
        public static void PauseAllMusic()
        {
            PauseAllAudio(Audio.AudioType.Music);
        }

        /// <summary>
        /// 모든 사운드 fx 일시 중지
        /// </summary>
        public static void PauseAllSounds()
        {
            PauseAllAudio(Audio.AudioType.Sound);
        }

        /// <summary>
        /// 모든 UI 사운드 fx 일시 중지
        /// </summary>
        public static void PauseAllUISounds()
        {
            PauseAllAudio(Audio.AudioType.UISound);
        }

        private static void PauseAllAudio(Audio.AudioType audioType)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioDict[key];
                audio.Pause();
            }
        }

        #endregion

        #region Resume Functions

        /// <summary>
        /// 모든 오디오 재개
        /// </summary>
        public static void ResumeAll()
        {
            ResumeAllMusic();
            ResumeAllSounds();
            ResumeAllUISounds();
        }

        /// <summary>
        /// 모든 음악 재개
        /// </summary>
        public static void ResumeAllMusic()
        {
            ResumeAllAudio(Audio.AudioType.Music);
        }

        /// <summary>
        /// 모든 사운드 fx 재개
        /// </summary>
        public static void ResumeAllSounds()
        {
            ResumeAllAudio(Audio.AudioType.Sound);
        }

        /// <summary>
        /// 모든 UI 사운드 fx 재개
        /// </summary>
        public static void ResumeAllUISounds()
        {
            ResumeAllAudio(Audio.AudioType.UISound);
        }

        private static void ResumeAllAudio(Audio.AudioType audioType)
        {
            Dictionary<int, Audio> audioDict = GetAudioTypeDictionary(audioType);

            List<int> keys = new List<int>(audioDict.Keys);
            foreach (int key in keys)
            {
                Audio audio = audioDict[key];
                audio.Resume();
            }
        }

        #endregion
    }
}
