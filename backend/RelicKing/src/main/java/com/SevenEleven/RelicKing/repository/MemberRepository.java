package com.SevenEleven.RelicKing.repository;

import java.util.Optional;

import org.springframework.data.jpa.repository.EntityGraph;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Modifying;
import org.springframework.data.jpa.repository.Query;

import com.SevenEleven.RelicKing.entity.Member;

public interface MemberRepository extends JpaRepository<Member, Integer> {

	@EntityGraph(attributePaths = {"memberRelics", "records"}, type = EntityGraph.EntityGraphType.FETCH)
	@Query("select m from Member m where m.memberId = :memberId")
	Optional<Member> findByMemberId(int memberId);

	Boolean existsByEmail(String email);

	Boolean existsByNickname(String nickname);

	Member findByEmail(String email);

	@Modifying
	@Query("update Member m set "
		+ "m.continuousLockDatePrev = m.continuousLockDate, "
		+ "m.continuousLockDate = case when m.todayLockTime < 3600 then 0 else m.continuousLockDate end, "
		+ "m.yesterdayLockTime = m.todayLockTime,"
		+ "m.todayLockTime = 0")
	void updateLockInfo();
}
