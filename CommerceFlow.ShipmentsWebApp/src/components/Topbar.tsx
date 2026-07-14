export default function Topbar() {
  return (
    <header className="topbar">
      <div className="search"><span aria-hidden="true">⌕</span><input aria-label="Search shipments" placeholder="Search shipment, driver, or destination" /><kbd>⌘ K</kbd></div>
      <div className="topbar-actions">
        <button type="button" className="icon-button" aria-label="Notifications">♢<b>3</b></button>
      </div>
    </header>
  );
}
