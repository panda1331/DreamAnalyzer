import '../styles/ConfirmModal.css';

function ConfirmModal({ isOpen, onConfirm, onCancel, message }) {
    if (!isOpen) return null;

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                <p>{message}</p>
                <div className="modal-buttons">
                    <button onClick={onConfirm}>Да</button>
                    <button onClick={onCancel}>Нет</button>
                </div>
            </div>
        </div>
    );
}

export default ConfirmModal;