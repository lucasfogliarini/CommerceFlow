import NotificationBell from "./NotificationBell";

export default function Topbar() {
  return (
    <header className="topbar">
      <div className="search"><span aria-hidden="true">⌕</span><input aria-label="Search shipments" placeholder="Search shipment, driver, or destination" /><kbd>⌘ K</kbd></div>
      <div className="topbar-actions">
        <NotificationBell />
      </div>
    </header>
  );
}
