import '../styles/Sandman.css';
import sandman from "../assets/sandman.png";
import { motion, AnimatePresence } from "motion/react";
import { useState } from 'react';

const dreamFacts = [
    "90% сновидений забываются в первые 10 минут после пробуждения.",
    "В снах мы видим только те лица, которые уже встречали в жизни.",
    "Слепые от рождения люди тоже видят сны, но они состоят из звуков, запахов и тактильных ощущений.",
    "Животные тоже видят сны. Во время фазы быстрого сна их мозг работает так же, как у человека.",
    "Мужчины и женщины видят сны по-разному: мужчинам чаще снятся незнакомцы и агрессия, а женщинам — знакомые люди и разговоры.",
    "Сонный паралич — это состояние, когда тело еще «спит» (заблокировано мозгом), а сознание уже проснулось.",
    "Раньше, до появления цветного ТВ, около 12% людей видели исключительно черно-белые сны.",
    "Во время сна наш мозг не отдыхает, а активно фильтрует и структурирует информацию за день."
]

function Sandman() {
    const [isOpen, setIsOpen] = useState(false);
    const [currentFact, setCurrentFact] = useState("");

    const handleSandmanClick = () => {
        if (!isOpen) {
            const randomIndex = Math.floor(Math.random() * dreamFacts.length);
            setCurrentFact(dreamFacts[randomIndex]);
            setIsOpen(true);
        } else {
            setIsOpen(false);
        }
    }

    return (
        <div className="sandman-wrapper">
            <AnimatePresence>
                {isOpen && (
                    <motion.div
                        className="dream-fact-bubble"
                        initial={{ opacity: 0, scale: 0.8, y: 20 }}
                        animate={{ opacity: 1, scale: 1, y: 0 }}
                        exit={{ opacity: 0, scale: 0.8, y: 20 }}
                        transition={{ duration: 0.3, ease: "easeOut" }}
                    >
                        <p>{currentFact}</p>
                        <span className="bubble-close-hint">кликни на меня, чтобы закрыть</span>
                    </motion.div>
                )}
            </AnimatePresence>
            <motion.div
                className="sandman-container"
                onClick={handleSandmanClick}
                animate={{ y: [0, -10, 0] }}
                transition={{
                    y: {
                        repeat: Infinity,
                        duration: 4,
                        ease: "easeInOut"
                    },
                }}
            >
                <motion.img
                    src={sandman}
                    alt="Sandman"
                    className="sandman-image"
                    initial={{ filter: "drop-shadow(0 0 10px #FFF33D)" }}
                    animate={{ filter: "drop-shadow(0 0 10px #FFF33D)" }}
                    transition={{ duration: 0.3 }}
                    whileHover={{
                        scale: 1.1,
                        filter: "drop-shadow(0 0 10px #FFF33D) drop-shadow(0 0 15px #8b5cf6) "
                    }}
                />
            </motion.div>
        </div>
    );
}

export default Sandman;