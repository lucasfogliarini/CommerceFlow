export default function Footer() {
  return (
    <footer className="footer">
      <div className="footer-inner">
        <div className="footer-logo">
          <span>⚡</span>
          <span>CommerceFlow</span>
        </div>
        <p className="footer-text">
          © {new Date().getFullYear()} CommerceFlow. Todos os direitos reservados.
        </p>
        <ul className="footer-links">
          <li>
            <a href="#">Termos</a>
          </li>
          <li>
            <a href="#">Privacidade</a>
          </li>
          <li>
            <a href="#">Suporte</a>
          </li>
        </ul>
      </div>
    </footer>
  );
}
