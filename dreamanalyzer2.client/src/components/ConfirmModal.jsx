import '../styles/ConfirmModal.css';
import { motion } from 'motion/react';

function ConfirmModal({ isOpen, onConfirm, onCancel, message }) {
    if (!isOpen) return null;

    return (
        <motion.div
            className="modal-overlay"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
        >
            <motion.div
                className="modal-content"
                initial={{ scale: 0.9, opacity: 0 }}
                animate={{ scale: 1, opacity: 1 }}
                transition={{ type: "spring", duration: 0.4, bounce: 0.2 }}
            >
                <p>{message}</p>
                <div className="modal-buttons">
                    <button onClick={onConfirm}>Да</button>
                    <button onClick={onCancel}>Нет</button>
                </div>
            </motion.div>
        </motion.div>
    );
}

export default ConfirmModal;