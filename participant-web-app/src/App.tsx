import { useReactMediaRecorder } from 'react-media-recorder'
import './styles/App.css'

function App() {
  const { status, error, startRecording, stopRecording } = useReactMediaRecorder({ screen: true })

  return (
    <div className="recorder-fullscreen">
      <p className="recording-status">{status}</p>
      {error && <p className="recording-error">{error}</p>}
      <div className="recorder-controls">
        <button type="button" onClick={startRecording}>
          Start Recording
        </button>
        <button type="button" onClick={stopRecording}>
          Stop Recording
        </button>
      </div>
    </div>
  )
}

export default App
