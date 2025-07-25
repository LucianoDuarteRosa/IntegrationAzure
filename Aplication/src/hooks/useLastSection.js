export function useLastSection() {
    const setLastSection = (section) => {
        localStorage.setItem('lastSection', section);
    };

    const getLastSection = () => {
        return localStorage.getItem('lastSection') || '/';
    };

    return { setLastSection, getLastSection };
}
