# Zollo

WinForms 기반 Ollama 연동 로컬 AI 코드 분석/질의 도구입니다.

Zollo는 로컬 PC 또는 사내망의 Ollama 서버를 사용해 C# 프로젝트를 분석하고, 일반 질문과 OCR 작업까지 수행할 수 있는 데스크톱 애플리케이션입니다. 외부 클라우드 API에 코드를 보내지 않고 로컬/사내망 환경에서 LLM 기반 개발 보조 기능을 사용하는 것을 목표로 합니다.

## 주요 기능

- 프로젝트 폴더 선택 및 파일 스캔
- C# 프로젝트 파일 선택 및 코드 분석
- Local Ollama / Remote Ollama 연결 지원
- 모델 목록 조회
- 연결 테스트 및 모델 워밍업
- `keep_alive` 적용으로 모델 재로딩 시간 감소
- Ollama 응답 성능 진단
- 일반 질문 모드
- OCR 모드
- 이미지 파일 스캔 및 vision 모델 기반 텍스트 추출
- 민감정보 의심 문자열 탐지
- 원격 Ollama 전송 전 보안 경고
- 답변 복사 및 저장
- 요청 취소 및 Timeout 처리

## 화면/모드 구성

### 코드 분석 모드

선택한 C# 프로젝트 파일을 Ollama에 전달해 코드 리뷰, 버그 가능성 분석, 리팩토링 제안, 보안 점검 등을 수행합니다.

분석 기준에는 컴파일 오류 가능성, 런타임 오류 가능성, 예외 처리 누락, async/await 오용, thread-safety 문제, WinForms UI Thread 문제, 메모리 누수, `IDisposable` 처리, 보안 문제, 입력값 검증, 유지보수성 등이 포함됩니다.

### 일반 질문 모드

파일 선택 없이 Ollama 모델에 일반 질문을 보낼 수 있습니다. 한국어 실무 답변용 모델로 `dev-ko` 사용을 권장합니다.

### OCR 모드

이미지 파일을 선택하거나 스캔된 파일 목록에서 이미지 파일을 선택해 이미지 내 텍스트를 추출합니다.

OCR 모드는 이미지 입력을 지원하는 Ollama vision 모델이 필요합니다. 예를 들어 `llava`, `qwen2.5vl`, `minicpm-v` 계열 모델을 사용할 수 있습니다.

## 기술 스택

- C#
- .NET 8
- Windows Forms
- Ollama HTTP API
- System.Text.Json
- async/await

## 요구 사항

- Windows
- .NET 8 SDK 또는 Runtime
- Ollama
- 코드 분석용 Ollama 모델
- OCR 사용 시 vision 모델

## Ollama 준비

Ollama가 실행 중이어야 합니다.

```powershell
ollama serve
```

사용 가능한 모델을 준비합니다.

```powershell
ollama pull qwen2.5-coder:14b
ollama pull llava
```

일반 질문용 커스텀 모델 예시:

```powershell
cd C:\ollama-models

@'
FROM qwen2.5-coder:14b

SYSTEM """
너는 한국어로 답하는 시니어 소프트웨어 엔지니어 AI다.

답변 원칙:
- 결론을 먼저 말한다.
- 실무적으로 바로 적용 가능한 답을 준다.
- 코드 예시는 실행 가능해야 한다.
- 버전, 전제조건, 한계를 명시한다.
- 불확실한 내용은 추측하지 말고 확인 필요하다고 말한다.
"""

PARAMETER temperature 0.2
PARAMETER top_p 0.9
PARAMETER repeat_penalty 1.1
PARAMETER num_ctx 8192
'@ | Set-Content -Path .\Modelfile -Encoding utf8

ollama create dev-ko -f .\Modelfile
```

## 실행 방법

프로젝트를 빌드합니다.

```powershell
dotnet build
```

실행합니다.

```powershell
dotnet run
```

Visual Studio에서 `zzollo.sln`을 열어 실행할 수도 있습니다.

## 성능 진단

Zollo는 Ollama `/api/chat` 응답에서 다음 성능 정보를 읽어 화면에 표시합니다.

- 전체 소요 시간
- 모델 로딩 시간
- 프롬프트 처리 시간
- 답변 생성 시간
- 프롬프트 토큰 수
- 생성 토큰 수

첫 요청이 느린 경우 대부분 모델 로딩 시간이 원인입니다. Zollo는 `keep_alive: "30m"`와 워밍업 요청을 사용해 이후 요청이 더 빠르게 처리되도록 돕습니다.

## 기본 요청 옵션

현재 기본 옵션은 다음과 같습니다.

```json
{
  "keep_alive": "30m",
  "stream": false,
  "options": {
    "temperature": 0.2,
    "num_ctx": 8192,
    "num_predict": 4096
  }
}
```

모드에 따라 `num_predict` 값은 다르게 적용됩니다.

| 모드 | num_predict |
| --- | ---: |
| 일반 질문 | 512 |
| 코드 분석 | 4096 |
| OCR | 2048 |
| 워밍업 | 8 |

## 보안 주의

Remote Ollama를 사용할 경우 선택한 코드, 질문, 이미지가 사내망의 다른 PC로 전송될 수 있습니다.

전송 전 다음 정보가 포함되어 있지 않은지 확인하세요.

- API Key
- Access Token
- 비밀번호
- DB 연결 문자열
- JWT Secret
- 개인정보
- 내부 시스템 주소

Zollo는 민감정보 의심 문자열을 탐지해 경고하지만, 모든 민감정보를 완벽하게 탐지하지는 않습니다.

## 프로젝트 구조

```txt
zzollo/
├─ Form1.cs              # 주요 UI 이벤트 및 기능 로직
├─ Form1.Designer.cs     # WinForms UI 구성
├─ OllamaClient.cs       # Ollama API 공통 클라이언트 및 DTO
├─ Program.cs            # 애플리케이션 진입점
├─ zzollo.csproj         # .NET 프로젝트 파일
└─ 요구사항_정의서.txt   # 초기 요구사항 문서
```

## GitHub 업로드 시 제외 권장

다음 파일/폴더는 저장소에 올리지 않는 것을 권장합니다.

```gitignore
.vs/
bin/
obj/
.dotnet-home/
*.user
```

## 향후 개선 아이디어

- 코드 분석 결과를 파일별로 캐싱
- Git diff 기반 변경 코드 리뷰
- Roslyn 기반 클래스/메서드 구조 분석
- 분석 프롬프트 템플릿 관리
- 모델별 옵션 UI 제공
- OCR 결과 후처리 및 저장
- 프로젝트별 설정 프로파일 저장

## License

라이선스는 저장소 공개 목적에 맞춰 선택하세요. 개인 포트폴리오 공개와 자유로운 재사용을 원한다면 MIT License를 권장합니다.
