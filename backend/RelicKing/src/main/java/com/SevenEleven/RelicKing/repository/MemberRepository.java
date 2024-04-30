package com.SevenEleven.RelicKing.repository;

import java.util.Optional;

import org.springframework.data.jpa.repository.EntityGraph;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import com.SevenEleven.RelicKing.entity.Member;

public interface MemberRepository extends JpaRepository<Member, Integer> {

	@EntityGraph(attributePaths = {"memberRelics", "records"}, type = EntityGraph.EntityGraphType.FETCH)
	@Query("select m from Member m where m.memberId = :memberId")
	Optional<Member> findByMemberId(int memberId);

	Boolean existsByEmail(String email);

	Boolean existsByNickname(String nickname);
}
